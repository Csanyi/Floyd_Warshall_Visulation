﻿using Floyd_Warshall.ViewModel.Commands.AlgorithmCommands;
using Floyd_Warshall_Model;
using Floyd_Warshall_Model.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Floyd_Warshall.ViewModel
{
    public class AlgorithmViewModel : ViewModelBase
    {
        private readonly GraphModel _graphModel;

        private readonly DispatcherTimer _timer;
        public DispatcherTimer Timer { get { return _timer; } }

        public int TimerInterval
        {
            get { return _timer.Interval.Seconds * 1000 + _timer.Interval.Milliseconds; }
            set
            {
                _timer.Interval = TimeSpan.FromMilliseconds(value);
                OnPropertyChanged();
            }
        }

        private int _size;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        public int? K => _graphModel.K;

        public bool IsEnoughVerteces { get { return _graphModel.GetVertexCount() > 1; } }

        public bool IsNegCycleFound { get; set; }

        private int? _vertexInNegCycle;
        public int? VertexInNegCycle
        {
            get { return _vertexInNegCycle; }
            set
            {
                _vertexInNegCycle = value;
                IsNegCycleFound = _vertexInNegCycle != null;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNegCycleFound));
            }
        }

        public bool IsInitialized => _graphModel.IsAlgorthmInitialized;

        public bool IsRunning => _graphModel.IsAlgorithmRunning;

        private bool _isStopped;
        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MatrixGridViewModel> D { get; set; }
        public ObservableCollection<MatrixGridViewModel> Pi { get; set; }
        public ObservableCollection<int> VertexIds { get; set; }

        public ICommand InitCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StepCommand { get; private set; }

        public AlgorithmViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            InitCommand = new AlgorithmInitCommand(this, graphModel);
            CancelCommand = new AlgorithmCancelCommand(this, graphModel);

            PauseCommand = new AlgorithmPauseCommand(this);
            StartCommand = new AlgorithmStartCommand(this);

            StepCommand = new AlgorithmStepCommand(this, graphModel);

            _timer = new DispatcherTimer();
            TimerInterval = 1000;
            _timer.Tick += Timer_Tick;

            _graphModel.VertexAdded += Model_VertexCntChanged;
            _graphModel.VertexRemoved += Model_VertexCntChanged;
            _graphModel.AlgorithmStarted += Model_AlgorithmStarted;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
            _graphModel.AlgorithmEnded += Model_AlgorithmEnded;
            _graphModel.NegativeCycleFound += Model_NegativeCycleFound;

            D = new ObservableCollection<MatrixGridViewModel>();
            Pi = new ObservableCollection<MatrixGridViewModel>();
            VertexIds = new ObservableCollection<int>();

            IsStopped = true;
            VertexInNegCycle = null;
        }

        public void CallPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _graphModel.StepAlgorithm();
        }

        private void Model_VertexCntChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnoughVerteces));
        }

        private void Model_AlgorithmStarted(object? sender, AlgorithmEventArgs e)
        {
            OnPropertyChanged(nameof(K));

            List<int> vertexIds = _graphModel.GetVertexIds();

            for(int i = 0; i < e.D.GetLength(0); ++i)
            {
                for(int j = 0; j < e.D.GetLength(1); ++j)
                {
                    int ind = i * e.D.GetLength(0) + j;

                    D.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.D[i, j],
                        X = vertexIds[i],
                        Y = vertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                    Pi.Add(new MatrixGridViewModel()
                    {
                        Index = ind,
                        Value = e.Pi[i, j],
                        X = vertexIds[i],
                        Y = vertexIds[j],
                        ClickCommand = new MatrixGridClickCommand(this, _graphModel),
                    });
                }
            }
        }
        
        private void Model_AlgorithmEnded(object? sender, EventArgs e)
        {
            _timer.Stop();
            IsStopped = true;
        }

        private void Model_AlgorithmStepped(object? sender, AlgorithmEventArgs e)
        {
            OnPropertyChanged(nameof(K));
            OnPropertyChanged(nameof(IsRunning));

            for(int i = 0; i < D.Count; ++i)
            {
                int j = i / e.D.GetLength(0);
                int k = i % e.D.GetLength(1);
                D[i].Value = e.D[j, k];
                Pi[i].Value = e.Pi[j, k];
            }
        }

        private void Model_NegativeCycleFound(object? sender, int e)
        {
            VertexInNegCycle = e;
        }
    }
}
