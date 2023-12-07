using Entidades.DataBase;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoPedidoEnCurso(IComestible menu);
    //Renombrar el delegado nuevo ingreso por delegado nuevo pedido(EJ DelegadoPedidoEnCurso).

    public class Cocinero<T> where T : IComestible, new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;

        private Task tarea;
        //private T pedidoEnPreparacion;

        public event DelegadoPedidoEnCurso OnPedido;
        public event DelegadoDemoraAtencion OnDemora;

        //
        private Mozo<T> mozo;
        private T pedidoEnPreparacion;
        private Queue<T> pedidos;
        //


        public Cocinero(string nombre)
        {
            this.nombre = nombre;
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            this.mozo.OnPedido += TomarNuevoPedido;

        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();

                }
                else
                {
                    this.mozo.EmpezarATrabajar = false;
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        //
        public Queue<T> Pedidos { get { return pedidos; } }
        //




        private void EmpezarACocinar()//InciarIngreso por EmpezarACocinar
        {
            tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    if (pedidos.Count > 0)
                    {
                        // Asignar a pedido en preparación el primer pedido de la lista de pedidos
                        T pedidoEnPreparacion = pedidos.Dequeue();
                        //NotificarNuevoIngreso();
                        EsperarProximoIngreso();
                        cantPedidosFinalizados++;
                        DataBaseManager.GuardarTicket<T>(nombre, pedidoEnPreparacion);
                    }
                }
            }, cancellation.Token);
        }
        //"borrado"
        //private void NotificarNuevoIngreso()
        //{
        //    if (OnPedido != null)
        //    {
        //        pedidoEnPreparacion = new T();
        //        pedidoEnPreparacion.IniciarPreparacion();
        //        OnPedido.Invoke(pedidoEnPreparacion);
        //    }

        //}
        private void EsperarProximoIngreso()
        {
            if (OnDemora != null)
            {
                int time = 0;

                while (!pedidoEnPreparacion.Estado && !cancellation.IsCancellationRequested)
                {
                    OnDemora.Invoke(time);
                    Thread.Sleep(1000);
                    time += 1;
                }
                demoraPreparacionTotal += time;

            }
        }
        //
        private void TomarNuevoPedido(T menu) 
        {
            if (OnPedido != null )
            {
                pedidos.Enqueue(menu);
            }
        }

    }
}
