﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Persistence
{
    public class GrapfFileDataAccess : IGraphDataAccess
    {
        public async Task<Tuple<Graph, IEnumerable<Tuple<Vertex, double, double>>>> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader= new StreamReader(path))
                {
                    string line = await reader.ReadLineAsync();
                    bool isDirected = Convert.ToBoolean(line);

                    Graph graph = new Graph(isDirected);
                    List<Tuple<Vertex, double, double>> locations = new List<Tuple<Vertex, double, double>>();

                    line = await reader.ReadLineAsync();

                    if(line != null && line != "")
                    {
                        string[] values = line.Split(' ');

                        foreach (string value in values)
                        {
                            string[] v = value.Split(';');
                            Vertex vertex = new Vertex(int.Parse(v[0]));
                            graph.AddVertex(vertex);
                            locations.Add(new Tuple<Vertex, double, double>(vertex, double.Parse(v[1]), double.Parse(v[2])));
                        }

                        line = await reader.ReadLineAsync();
                        if (line != null && line != "")
                        {
                            values = line.Split(' ');

                            foreach (string value in values)
                            {
                                string[] e = value.Split(';');
                                Vertex from = graph.GetVertexById(int.Parse(e[0]));
                                Vertex to = graph.GetVertexById(int.Parse(e[1]));
                                if (graph.GetEdge(from, to) == null)
                                {
                                    graph.AddEdge(from, to, int.Parse(e[2]));
                                }
                            }
                        }
                    }

                    return new Tuple<Graph, IEnumerable<Tuple<Vertex, double, double>>>(graph, locations);
                }
            } catch
            {
                throw new GraphDataException();
            }
        }

        public async Task SaveAsync(string path, Graph graph, IEnumerable<Tuple<Vertex, double, double>> locations)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(graph.IsDirected.ToString());

                    string delimiter = "";

                    foreach(var v in locations)
                    {
                        await writer.WriteAsync(delimiter + v.Item1.Id + ";" + v.Item2 + ";" + v.Item3);
                        delimiter = " ";
                    }
                    await writer.WriteLineAsync();

                    delimiter = "";

                    foreach(Edge e in graph.GetEdges())
                    {
                        await writer.WriteAsync(delimiter + e.From.Id + ";" + e.To.Id + ";" + e.Weight);
                        delimiter = " ";
                    }
                }
            } catch
            {
                throw new GraphDataException();
            }
        }
    }
}