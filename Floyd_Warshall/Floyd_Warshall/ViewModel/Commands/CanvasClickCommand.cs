﻿using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class CanvasClickCommand : CommandBase
    {
        private GraphCanvasViewModel _vm;
        private GraphModel _graphModel;

        public CanvasClickCommand(GraphCanvasViewModel graphCanvasViewModel, GraphModel graphModel) 
        {
            _vm = graphCanvasViewModel;
            _graphModel = graphModel;
        }

        public override void Execute(object parameter)
        {
            Vertex v = new Vertex(_vm.GetVertexId);

            VertexViewModel vertex = new VertexViewModel(v)
            {
                CanvasX = _vm.MouseX - VertexViewModel.Size / 2,
                CanvasY = _vm.MouseY - VertexViewModel.Size / 2,
                IsSelected = false,
                RightClickCommand = new VertexRightClickCommand(_vm, _graphModel),
                LeftClickCommand = new VertexLeftClickCommand(_vm, _graphModel),
            };

            _graphModel.Graph.AddVertex(v);
            _vm.Verteces.Add(vertex);
            _vm.Views.Add(vertex);
        }
    }
}