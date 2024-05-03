namespace acunatp1
{
public class Program()
    {
        public static void Correr()
        {
            var juego = new Juego();
            juego.CorrerJuego();
        }
    }

    class Juego
    {
        int frame;
        Personaje jugador;
        List<Personaje> bicho;
        Arma arma;
        List<Arma> disparo;
        Habitacion habitacion;
        Random rand;



        public void CorrerJuego()
        {
            
            Inicializacion();

            DibujarPantalla();

            while (true)
            {
                
                ConsoleKeyInfo? input = null;
                if (Console.KeyAvailable)
                    input = Console.ReadKey();

                ActualizarDatos(input);

                DibujarPantalla();

                Thread.Sleep(1000);
            }
        }

        void Inicializacion()
        {
            frame = 0;
            habitacion = new Habitacion(26, 15);
            jugador = new Personaje(13, 12, habitacion, '^');
            rand = new Random();

            bicho = new List<Personaje>();

            for (int fila = 0; fila < 3; fila++)
            {
                for (int columna = 0; columna < 3; columna++)
                {
                    int x = 12 + columna * 3; 
                    int y = 2 + fila * 3; 
                    
                    bicho.Add(new Personaje(x, y, habitacion, '='));
                }
            }
            
            disparo = new List<Arma>();

        }

        void ActualizarDatos(ConsoleKeyInfo? input)
        {

            frame++;

            if (input.HasValue)
            {
                var tecla = input.Value.Key;

                if (tecla == ConsoleKey.RightArrow)
                    jugador.MoverHacia(1, 0);
                if (tecla == ConsoleKey.LeftArrow)
                    jugador.MoverHacia(-1, 0);
                if(tecla == ConsoleKey.Spacebar)
                    Disparar();
                                        
                if(tecla == ConsoleKey.Escape)
                    Environment.Exit(0);
            }

            foreach (var bicho in bicho)
            {
                bicho.MoverHacia(rand.Next(-1, 2), 0);
                            
            }

            foreach (var disparo in disparo)
            {
                disparo.MoverHacia(0, -1);               

            }

            void Disparar()
            {
                disparo.Add(new Arma(jugador.x, jugador.y -1, habitacion, '|', bicho));
            }

            for (int i = disparo.Count - 1; i >= 0; i--)
            {
                if (disparo[i].y == 1)
                {
                    disparo.RemoveAt(i);
                }

                else
                {
                    for (int b = 0; b < bicho.Count; b++)
                    {
                        if (disparo[i].ColisionaConBicho(bicho[b]))
                        {
                            disparo.RemoveAt(i);
                            bicho.RemoveAt(b);
                            break;
                        }

                    }
                }  
            }

        }

        void DibujarPantalla()
        {
            Lienzo lienzo = new Lienzo(26, 15);
            habitacion.Dibujar(lienzo);
            jugador.Dibujar(lienzo);

            foreach (var bichoPersonaje in bicho)
            {
                bichoPersonaje.Dibujar(lienzo);
            }

            foreach (var disparoArma in disparo)
            {
                disparoArma.Dibujar(lienzo);
            }

            lienzo.MostrarEnPantalla();
            Console.WriteLine($"Frame: {frame}");
        }
    }

    class Personaje
    {
        public int x;
        public int y;
        public List<Personaje> bicho;
        private IMapa mapa;
        private char dibujo;

        public Personaje(int x, int y, IMapa mapa, char dibujo)
        {
            this.x = x;
            this.y = y;
            this.mapa = mapa;
            this.dibujo = dibujo;
            this.bicho = new List<Personaje>();

        }

        public void MoverHacia(int x, int y)
        {
            var nuevoX = this.x + x;
            var nuevoY = this.y + y;

            if (mapa.EstaLibre(nuevoX, nuevoY))
            {
                this.x = nuevoX;
                this.y = nuevoY;
            }
        }

        public void Dibujar(Lienzo lienzo)
        {
            lienzo.Dibujar(x, y, dibujo);
        }
    }

    class Arma : Personaje
    {
        public List<Personaje> bicho;
        public Arma(int x, int y, IMapa mapa, char dibujo, List<Personaje> bicho) : base(x, y, mapa, dibujo)
        {
            this.bicho = bicho;
        }

        public bool ColisionaConBicho(Personaje personaje)
        {
            foreach (var bicho in bicho)
            {
                if (this.x == bicho.x && this.y == bicho.y)
                {
                    return true;
                }
            }
            
            return false;
        }

    }

    class Lienzo
    {
        private char[,] celdas;
        private int ancho, alto;

        public Lienzo(int ancho, int alto)
        {
            this.ancho = ancho;
            this.alto = alto;
            celdas = new char[ancho, alto];
        }

        public void Dibujar(int x, int y, char celda)
        {
            celdas[x, y] = celda;
        }

        public void MostrarEnPantalla()
        {
      
            Console.Clear();

            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    Console.Write(celdas[x, y]);
                }
                Console.Write("\n");
            }
        }
    }

    interface IMapa
    {
        bool EstaLibre(int x, int y);
    }

    class Habitacion : IMapa
    {
        private List<Fila> filas;

        public Habitacion(int ancho, int alto)
        {
        
            filas = new List<Fila>();

            filas.Add(new FilaBorde(ancho));
            for (int fila = 1; fila < alto - 1; fila++)
            {
                filas.Add(new FilaMedia(ancho));
            }
            filas.Add(new FilaBorde(ancho));
        }

        public void Dibujar(Lienzo lienzo)
        {
            for (int y = 0; y < filas.Count(); y++)
            {
                filas[y].Dibujar(lienzo, y);
            }
        }

        public bool EstaLibre(int x, int y)
        {
            return filas[y].EstaLibre(x);
        }
    }

    abstract class Fila
    {
        protected List<char> celdas;

        public Fila(int cantidadCeldas)
        {
            this.celdas = new List<char>();

            AgregarPunta();
            for (int i = 1; i < cantidadCeldas - 1; i++)
            {
                AgregarMedio();
            }
            AgregarPunta();
        }

        protected abstract void AgregarMedio();
        protected abstract void AgregarPunta();

        public void Dibujar(Lienzo lienzo, int y)
        {
            for (int x = 0; x < celdas.Count(); x++)
            {
                lienzo.Dibujar(x, y, celdas[x]);
            }
        }

        internal bool EstaLibre(int x)
        {
            return celdas[x] == ' ';
        }
    }

    class FilaMedia : Fila
    {
        public FilaMedia(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add(' ');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }

    class FilaBorde : Fila
    {
        public FilaBorde(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add('#');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }
}
