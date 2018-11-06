using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho_Grafos
{
    class Grafo_Und : Grafo
    {
        public Grafo_Und(int numeroDeVertices, List<ParOrdenado> listaDePares, string [] vetorRotulos)
        {
            Vertices = new Vertice[numeroDeVertices];

            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i] = new Vertice(i, vetorRotulos[i]);


            foreach (ParOrdenado parOrdenado in listaDePares)
                FormarNovaAresta(Vertices[parOrdenado.X].ID, Vertices[parOrdenado.Y].ID, parOrdenado.Pesos, parOrdenado.ListaDeHorarios);
        }

        public Grafo_Und(int numeroDeVertices, List<ParOrdenado> listaDePares)
        {
            Vertices = new Vertice[numeroDeVertices];

            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i] = new Vertice(i);


            foreach (ParOrdenado parOrdenado in listaDePares)
                FormarNovaAresta(Vertices[parOrdenado.X].ID, Vertices[parOrdenado.Y].ID, parOrdenado.Pesos, parOrdenado.ListaDeHorarios);
        }

        public override bool isAdjacente(Vertice v1, Vertice v2)
        {
            for (int k = 0; k < Vertices[v1.ID].ListaDeAdjacencia.Count; k++)
            {
                if (Vertices[v1.ID].ListaDeAdjacencia[k].verticeDestino.ID == v2.ID)
                    return true;
            }

            return false;
        }

        public override int getGrau(Vertice v1)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (v1 == Vertices[i])
                    return Vertices[i].ListaDeAdjacencia.Count;
            }

            return -1;
        }

        public bool isCompleto()
        {
            return isCompleto(0);
        }

        private bool isCompleto(int indice)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i] != Vertices[indice])
                {
                    if (!isAdjacente(Vertices[indice], Vertices[i]))
                    {
                        return false;
                    }
                }
            }

            indice++;

            if (indice < Vertices.Length)
                return isCompleto(indice);
            else
                return true;
        }

        public override Grafo getComplementar()
        {
            Grafo_Und grafo;
            int nVertices;
            List<ParOrdenado> pares = new List<ParOrdenado>();
            for (int i = 0; i < Vertices.Length; i++)
            {
                for (int j = 0; j < Vertices.Length; j++)
                {
                    if (i != j)
                    {
                        if (!isAdjacente(Vertices[i], Vertices[j]))
                        {
                            if (!VerificarParExistente(Vertices[i].ID, Vertices[j].ID, pares))
                            {
                                ParOrdenado par = new ParOrdenado(Vertices[i].ID, Vertices[j].ID);
                                pares.Add(par);
                            }
                        }
                    }
                }
            }

            nVertices = Vertices.Length;
            return grafo = new Grafo_Und(nVertices, pares);
        }

        private bool VerificarParExistente(int x, int y, List<ParOrdenado> lista)
        {
            for (int h = 0; h < lista.Count; h++)
            {
                if ((lista[h].X == x && lista[h].Y == y) || (lista[h].Y == x && lista[h].X == y))
                    return true;
            }

            return false;
        }

        public override bool isEuleriano()
        {
            if (!isConexo())
                return false;

            for (int i = 0; i < Vertices.Length; i++)
            {
                if (getGrau(Vertices[i]) % 2 != 0)
                    return false;
            }

            return true;
        }

        public override string ListaDeAdjacencia()
        {
            string valor = "";

            for (int i = 0; i < Vertices.Length; i++)
            {
                valor += "Aeroporto " + Vertices[i].Rotulo + ":";

                for (int j = 0; j < Vertices[i].ListaDeAdjacencia.Count; j++)
                {
                    Aresta a = Vertices[i].ListaDeAdjacencia[j];

                    valor += (a.verticeDestino.Rotulo) + ":" + a.Pesos.Distancia;

                    if (j != Vertices[i].ListaDeAdjacencia.Count - 1)
                        valor += ", ";

                    else
                        valor += "\n \n";
                }
            }

            return valor;
        }

        public Grafo_Und Kruskal()
        {
            //algoritmo só deve ser executado em grafos conexos
            if (!isConexo()) //se o grafo não é conexo, o algoritmo não será executado e retornará null
                return null;

            //pré processamento dos dados
            List<Aresta> listaArestas = new List<Aresta>();
            List<ParOrdenado> pares = new List<ParOrdenado>();
            Grafo_Und grafoAuxilir = new Grafo_Und(Vertices.Length, pares);

            for (int a = 0; a < grafoAuxilir.Vertices.Length; a++)
                grafoAuxilir.Vertices[a].Rotulo = this.Vertices[a].Rotulo;

            for (int g = 0; g < Vertices.Length; g++)
            {
                for (int a = 0; a < Vertices[g].ListaDeAdjacencia.Count; a++)
                {
                    if (Vertices[g].ListaDeAdjacencia[a].Direcao == 1)
                        listaArestas.Add(Vertices[g].ListaDeAdjacencia[a]);
                }
            }

            Aresta[] arestasOrdenadas = insertionSort(listaArestas.ToArray(),0);
            //dados processados e arestas já ordenadas

            //para cada aresta no vetor de aresta
            for (int v = 0; v < arestasOrdenadas.Length; v++)
            {
                //capturar origem e destino da aresta, ignorando a direção, pois é um grafo não dirigido
                int idOrigem = arestasOrdenadas[v].verticeOrigem.ID;
                int idDestino = arestasOrdenadas[v].verticeDestino.ID;

                //if para ignorar os loops do grafo original
                if (idOrigem != idDestino)
                {
                    //fila auxiliar para executar a busca pelo ciclo
                    Queue<int> fila = new Queue<int>();

                    //verifica se a adição da nova aresta vai gerar um ciclo
                    if (!VerificarCiclo(fila, idOrigem, idDestino, idDestino, grafoAuxilir))
                        grafoAuxilir.FormarNovaAresta(idOrigem, idDestino, arestasOrdenadas[v].Pesos); //não formando ciclo, nova aresta é criada
                }
            }

            return grafoAuxilir;
        }

        private bool VerificarCiclo(Queue<int> fila, int idOrigem, int idDestino, int idAtual, Grafo_Und grafoAux)
        {
            //MÉTODO RECURSIVO BASEADO NA TRAVESSIA EM AMPLITUDE
            grafoAux.Vertices[idAtual].EstadoCor = 2;

            //para cada item da lista de adjacencia do vértice atual
            for (int w = 0; w < grafoAux.Vertices[idAtual].ListaDeAdjacencia.Count; w++)
            {
                //captura o indice do vertice destino
                int idLaco = grafoAux.Vertices[idAtual].ListaDeAdjacencia[w].verticeDestino.ID;

                //se o indice capturado for igual a origem, singnifica que esses componentes ja são conexos
                //a adição de uma nova aresta, formaria um ciclo, portanto, retorna verdadeiro
                if (idLaco == idOrigem)
                {
                    //reseta as cores do grafo auxiliar, para não atrapalhar as próximas execuções do algoritmo
                    grafoAux.ResetarCores();
                    return true;
                }

                else
                {
                    //verifica se o vertice já não foi visitado anteriormente
                    if (grafoAux.Vertices[idLaco].EstadoCor == 1)
                    {
                        fila.Enqueue(idLaco); //enfileira o indice do vertice que está sendo visitado
                        grafoAux.Vertices[idLaco].EstadoCor = 2; //pinta o vertice de azul
                    }
                }
            }

            grafoAux.Vertices[idAtual].EstadoCor = 3; //pinta o vertice de vermelho

            //condição para chamada recursiva
            //se existem itens na fila, ainda há vertices para verificar a condição de ciclo
            if (fila.Count > 0)
            {
                //remove da fila, detectando o próximo vertice a ser verificado
                int prox = fila.Dequeue();

                //chamada recursiva com parametros atualizados
                return VerificarCiclo(fila, idOrigem, idDestino, prox, grafoAux);
            }

            //else que será executado quando todos os vértices já tiverem sido visitados
            else
            {
                //reseta as cores do grafo auxiliar, para não atrapalhar as próximas execuções do algoritmo
                grafoAux.ResetarCores();
                return false;
            }
        }

        protected override void Dijkstra(int[] vetorDistancias, int[] vetorPredecessor, int atual, int nPeso, Horario horarioAtual)
        {
            int idMenor = -1;

            Vertices[atual].EstadoCor = 3;

            //para todo item da lista de adjacencia, com direção 1
            for (int f = 0; f < Vertices[atual].ListaDeAdjacencia.Count; f++)
            {
                int indexDestino = Vertices[atual].ListaDeAdjacencia[f].verticeDestino.ID;

                if (Vertices[indexDestino].EstadoCor == 1)
                {
                    int dist = 0;

                    if (nPeso == 0)
                    {
                        dist = Vertices[atual].ListaDeAdjacencia[f].Pesos.Distancia + vetorDistancias[atual];
                    }

                    else if (nPeso == 1)
                    {
                        dist = Vertices[atual].ListaDeAdjacencia[f].Pesos.DuracaoDoVoo + vetorDistancias[atual];
                    }

                    else if (nPeso == 3)
                    {
                        int tempoEmEspera = Horario.CalcularTempoEmEspera(Vertices[atual].ListaDeAdjacencia[f].ListaDeVoos, horarioAtual);
                        dist = Vertices[atual].ListaDeAdjacencia[f].Pesos.DuracaoDoVoo + vetorDistancias[atual] + tempoEmEspera;
                        horarioAtual.Minuto += tempoEmEspera;
                    }

                    if (dist < vetorDistancias[indexDestino])
                    {
                        vetorPredecessor[indexDestino] = atual;
                        vetorDistancias[indexDestino] = dist;
                    }
                }
            }

            idMenor = BuscarVerticeAberto(vetorDistancias);

            if (idMenor != -1)
                Dijkstra(vetorDistancias, vetorPredecessor, idMenor, nPeso, horarioAtual);
        }

        protected override void TravessiaEmAplitude(int distancia, int tempo, int atual, Queue<int> fila)
        {
            for (int w = 0; w < Vertices[atual].ListaDeAdjacencia.Count; w++)
            {
                int indice = Vertices[atual].ListaDeAdjacencia[w].verticeDestino.ID;

                if (Vertices[indice].EstadoCor == 1)
                {
                    Vertices[indice].EstadoCor = 2;
                    Vertices[indice].Predecessor = Vertices[atual];
                    Vertices[indice].TempoDeDescoberta = tempo;
                    tempo++;
                    fila.Enqueue(indice);
                }
            }

            Vertices[atual].TempoDeFinalizacao = tempo;
            Vertices[atual].EstadoCor = 3;
            tempo++;

            if (fila.Count > 0)
            {
                int novoIndice = fila.Dequeue();
                TravessiaEmAplitude(distancia, tempo, novoIndice, fila);
            }
        }

        public string ComponentesConexos()
        {
            ResetarCores();
            string val = "";
            return ComponentesConexos(0, val, 0, 1, 1);
        }

        public string ComponentesConexos(int atual, string valor, int tempo, int distancia,int nConjuntos)
        {
            tempo++;
            Vertices[atual].TempoDeDescoberta = tempo;
            Vertices[atual].EstadoCor = 2;
            valor += Vertices[atual].ID + ", ";
            for (int v = 0; v < Vertices[atual].ListaDeAdjacencia.Count; v++)
            {
                int destino = Vertices[atual].ListaDeAdjacencia[v].verticeDestino.ID;

                if (Vertices[destino].EstadoCor == 1)
                {
                    tempo++;
                    Vertices[destino].Predecessor = Vertices[atual];
                    Vertices[destino].Distancia = distancia;
                    Vertices[destino].TempoDeDescoberta = tempo;
                   return ComponentesConexos(destino, valor, tempo, distancia+1, nConjuntos);
                }
            }

            tempo++;
            Vertices[atual].EstadoCor = 3;


            if (Vertices[atual].Predecessor == null)
            {
                int novoIndice = ExisteVerticesEmBranco();

                if (novoIndice != -1)
                {
                    nConjuntos++;
                    valor += "\nConjunto " + nConjuntos + ": ";
                    return ComponentesConexos(novoIndice, valor, tempo, distancia + 1, nConjuntos);
                }

                else
                {
                    return valor;
                }
            }

            else
            {
               return ComponentesConexos(Vertices[atual].Predecessor.ID, valor, tempo, distancia + 1, nConjuntos);
            }
        }
    }
}
