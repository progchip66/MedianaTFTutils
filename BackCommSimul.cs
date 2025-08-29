using System;
using System.ComponentModel;
using System.Threading;
using COMMAND;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator
{
    public sealed class BackCommSimul : IDisposable
    {
        private readonly BackgroundWorker _bw = new BackgroundWorker();
        private readonly Func<ECommand, Efl_DEV, byte[], int, byte[]> _exec;
        private Timer _timer;

        private bool _run;
        private readonly int _intervalMs;

        private ECommand _lastCmd;
        private Efl_DEV _lastDev;
        private byte[] _lastData;
        private int _lastTimeout;

        private readonly SynchronizationContext _syncContext;

        private sealed class CommArgs
        {
            public ECommand Command;
            public Efl_DEV RecDev;
            public byte[] Data;
            public int Timeout;
        }

        /// <summary>
        /// Срабатывает после успешного завершения обмена (на UI-потоке).
        /// </summary>
        public event Action<byte[]> ResultReady;

        /// <summary>
        /// Ошибка фонового обмена (на UI-потоке).
        /// </summary>
        public event Action<Exception> Error;

        /// <param name="exec">
        /// Делегат на вашу функцию обмена:
        /// byte[] CommSendAnsv(ECommand, Efl_DEV, byte[] data, int timeout)
        /// </param>
        /// <param name="intervalMs">Интервал между циклами, мс (например, 1000)</param>
        public BackCommSimul(Func<ECommand, Efl_DEV, byte[], int, byte[]> exec, int intervalMs = 1000)
        {
            if (exec == null) throw new ArgumentNullException("exec");
            _exec = exec;
            _intervalMs = intervalMs;

            _bw.DoWork += DoWork;
            _bw.RunWorkerCompleted += Completed;

            // Сохраняем текущий контекст синхронизации (UI-поток WinForms)
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <summary>
        /// Универсальный старт/стоп.
        /// start = true: запоминаем параметры и запускаем цикл.
        /// start = false: останавливаем цикл.
        /// </summary>
        public void Control(bool start,
                            ECommand command = default(ECommand),
                            Efl_DEV recDev = Efl_DEV.fld_none,
                            byte[] data = null,
                            int timeout = 50)
        {
            if (start)
            {
                _lastCmd = command;
                _lastDev = recDev;
                _lastData = data;
                _lastTimeout = timeout;

                _run = true;
                StartOnce();
            }
            else
            {
                _run = false;
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void StartOnce()
        {
            if (_bw.IsBusy) return;

            var args = new CommArgs
            {
                Command = _lastCmd,
                RecDev = _lastDev,
                Data = _lastData,
                Timeout = _lastTimeout
            };

            _bw.RunWorkerAsync(args);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var a = (CommArgs)e.Argument;
            e.Result = _exec(a.Command, a.RecDev, a.Data, a.Timeout);
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _syncContext.Post(_ => Error?.Invoke(e.Error), null);
                return;
            }

            var ansv = e.Result as byte[];
            _syncContext.Post(_ => ResultReady?.Invoke(ansv), null);

            if (_run)
            {
                // Перезапускаем таймер — один тик через _intervalMs
                if (_timer == null)
                    _timer = new Timer(TimerTick, null, _intervalMs, Timeout.Infinite);
                else
                    _timer.Change(_intervalMs, Timeout.Infinite);
            }
        }

        private void TimerTick(object state)
        {
            if (_run)
                StartOnce();
        }

        public void Dispose()
        {
            _run = false;

            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }

            _bw.DoWork -= DoWork;
            _bw.RunWorkerCompleted -= Completed;
        }
    }

}
