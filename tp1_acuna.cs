namespace tp1
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
        Personaje jugador, bicho;
        Habitacion habitacion;
        Random rand;

        public void CorrerJuego()
        {
            // Inicializacion
            Inicializacion();

            // Dibujo el primer cuadro
            DibujarPantalla();

            // Game Loop!
            while (true)
            {
                // Escucho Input
                ConsoleKeyInfo? input = null;
                if (Console.KeyAvailable)
                    input = Console.ReadKey();

                // Actualizo Datos
                ActualizarDatos(input);

                // Dibujo Pantalla
                DibujarPantalla();

                Thread.Sleep(1000);
            }
        }

        void Inicializacion()
        {
            frame = 0;
            habitacion = new Habitacion(15, 10);
            jugador = new Personaje(2, 2, habitacion, '^');
            bicho = new Personaje(8, 5, habitacion, '=');
            rand = new Random();
        }

        void ActualizarDatos(ConsoleKeyInfo? input)
        {
            // Actualizo el frame
            frame++;

            if (input.HasValue)
            {
                var tecla = input.Value.Key;

                // Muevo al jugador
                if (tecla == ConsoleKey.RightArrow)
                    jugador.MoverHacia(1, 0);
                if (tecla == ConsoleKey.LeftArrow)
                    jugador.MoverHacia(-1, 0);
            }

            // Muevo al goblin (es un bicho)
            bicho.MoverHacia(rand.Next(-1, 2), rand.Next(-1, 2));
        }

        void DibujarPantalla()
        {
            Lienzo lienzo = new Lienzo(15, 10);
            habitacion.Dibujar(lienzo);
            jugador.Dibujar(lienzo);
            bicho.Dibujar(lienzo);

            lienzo.MostrarEnPantalla();
            Console.WriteLine($"Frame: {frame}");
        }
    }

    class Personaje
    {
        private int x, y;
        private IMapa mapa;
        private char dibujo;

        public Personaje(int x, int y, IMapa mapa, char dibujo)
        {
            this.x = x;
            this.y = y;
            this.mapa = mapa;
            this.dibujo = dibujo;
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
            // Limpio la consola antes de dibujar el nuevo cuadro
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
            // Inicializo filas
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
