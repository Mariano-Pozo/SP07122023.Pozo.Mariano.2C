using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoNuevoIngreso(IComestible menu);

    public class Cocinero<T> where T : IComestible , new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;

        private Task tarea;
        private T menu;

        public event DelegadoNuevoIngreso OnIngreso;
        public event DelegadoDemoraAtencion OnDemora;


        public Cocinero(string nombre)
        {
            this.nombre = nombre;
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
                    this.IniciarIngreso();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        

        

        

        private void IniciarIngreso()
        {
            tarea = Task.Run(() =>
            {
                while (cancellation.IsCancellationRequested.Equals(false))
                {
                    NotificarNuevoIngreso();
                    EsperarProximoIngreso();
                    cantPedidosFinalizados++;
                    DataBaseManager.GuardarTicket<T>(nombre, menu);
                }
            }, cancellation.Token);
        }

        private void NotificarNuevoIngreso()
        {
            if (OnIngreso is not null)
            {
                menu = new T();
                menu.IniciarPreparacion();
                OnIngreso.Invoke(menu);
            }

        }
        private void EsperarProximoIngreso()
        {
            if (OnDemora != null)
            {
                int tiempoEspera = 0;

                while (!menu.Estado && !cancellation.IsCancellationRequested)
                {
                    OnDemora.Invoke(tiempoEspera);
                    Thread.Sleep(1000);
                    tiempoEspera += 1;
                }
                demoraPreparacionTotal += tiempoEspera;

            }
        }
    }
}
