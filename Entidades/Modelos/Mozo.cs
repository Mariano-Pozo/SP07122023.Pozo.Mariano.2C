using Entidades.DataBase;
using Entidades.Files;
using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoPedido<T>(T menu);
    public class Mozo<T> where T : IComestible, new()
    {
        public event DelegadoNuevoPedido<T> OnPedido;

        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;

        public bool EmpezarATrabajar 
        {
            get
            {
                return tarea != null &&
                       (tarea.Status == TaskStatus.Running ||
                        tarea.Status == TaskStatus.WaitingToRun ||
                        tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && (tarea == null || tarea.Status != TaskStatus.Running ||
                             tarea.Status != TaskStatus.WaitingToRun ||
                             tarea.Status != TaskStatus.WaitingForActivation))
                {
                    cancellation = new CancellationTokenSource();
                    TomarPedidos();
                }
                else
                {
                    if (cancellation != null)
                    {
                        cancellation.Cancel();
                    }
                }
            }
        }


        private void TomarPedidos()
        {
            while (!cancellation.IsCancellationRequested)
            {
                NotificarNuevoPedido();
                Thread.Sleep(5000);
            }
        }

        private void NotificarNuevoPedido()
        {
            if (OnPedido != null)
            {
                T nuevoMenu = new T();
                menu.IniciarPreparacion();
                OnPedido.Invoke(nuevoMenu);
            }
            
        }
        
    }
}
