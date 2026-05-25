using System;
using System.Collections.Generic;
using System.Text;

namespace JuegoEstrategiaTablero
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            SistemaPrincipal sistema = new SistemaPrincipal();
            sistema.Ejecutar();
        }
    }

    #region Enumeraciones y Estructuras de Datos

    public enum TipoPieza { Peon, Caballo, Torre, Alfil, Reina, Rey }
    public enum IdJugador { Esmeralda = 1, Rubi = 2 }

    public readonly record struct Posicion(int X, int Y)
    {
        public bool EsValida() => X >= 0 && X < 8 && Y >= 0 && Y < 8;
    }

    public readonly record struct RegistroMovimiento(
        IdJugador Jugador,
        TipoPieza TipoDePieza,
        Posicion Desde,
        Posicion Hacia,
        bool EsCaptura,
        TipoPieza? TipoCapturado);

    #endregion

    #region Patrón Strategy (Movimiento de Piezas)

    public interface IEstrategiaMovimientoPieza
    {
        bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero);
    }

    public sealed class EstrategiaMovimientoPeon : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;

            Pieza? peon = tablero.ObtenerPieza(desde);
            if (peon == null) return false;

            int direccionAvance = (peon.Propietario == IdJugador.Esmeralda) ? 1 : -1;
            int deltaX = hacia.X - desde.X;
            int deltaY = hacia.Y - desde.Y;

            Pieza? piezaDestino = tablero.ObtenerPieza(hacia);

            if (deltaX == 0 && deltaY == direccionAvance)
            {
                return piezaDestino == null;
            }

            if (Math.Abs(deltaX) == 1 && deltaY == direccionAvance)
            {
                return piezaDestino != null && piezaDestino.Propietario != peon.Propietario;
            }

            return false;
        }
    }

    public sealed class EstrategiaMovimientoCaballo : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;

            Pieza? caballo = tablero.ObtenerPieza(desde);
            if (caballo == null) return false;

            int deltaX = Math.Abs(hacia.X - desde.X);
            int deltaY = Math.Abs(hacia.Y - desde.Y);

            if ((deltaX == 1 && deltaY == 2) || (deltaX == 2 && deltaY == 1))
            {
                Pieza? piezaDestino = tablero.ObtenerPieza(hacia);
                return piezaDestino == null || piezaDestino.Propietario != caballo.Propietario;
            }

            return false;
        }
    }

    public sealed class EstrategiaMovimientoTorre : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;
            if (desde.X == hacia.X && desde.Y == hacia.Y) return false;

            Pieza? torre = tablero.ObtenerPieza(desde);
            if (torre == null) return false;

            int deltaX = hacia.X - desde.X;
            int deltaY = hacia.Y - desde.Y;

            if (deltaX != 0 && deltaY != 0) return false;

            int stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
            int stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

            int x = desde.X + stepX;
            int y = desde.Y + stepY;

            while (x != hacia.X || y != hacia.Y)
            {
                if (tablero.ObtenerPieza(new Posicion(x, y)) != null)
                    return false;
                x += stepX;
                y += stepY;
            }

            Pieza? piezaDestino = tablero.ObtenerPieza(hacia);
            return piezaDestino == null || piezaDestino.Propietario != torre.Propietario;
        }
    }

    public sealed class EstrategiaMovimientoAlfil : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;
            if (desde.X == hacia.X && desde.Y == hacia.Y) return false;

            Pieza? alfil = tablero.ObtenerPieza(desde);
            if (alfil == null) return false;

            int deltaX = hacia.X - desde.X;
            int deltaY = hacia.Y - desde.Y;

            if (Math.Abs(deltaX) != Math.Abs(deltaY)) return false;

            int stepX = deltaX > 0 ? 1 : -1;
            int stepY = deltaY > 0 ? 1 : -1;

            int x = desde.X + stepX;
            int y = desde.Y + stepY;

            while (x != hacia.X || y != hacia.Y)
            {
                if (tablero.ObtenerPieza(new Posicion(x, y)) != null)
                    return false;
                x += stepX;
                y += stepY;
            }

            Pieza? piezaDestino = tablero.ObtenerPieza(hacia);
            return piezaDestino == null || piezaDestino.Propietario != alfil.Propietario;
        }
    }

    public sealed class EstrategiaMovimientoReina : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;
            if (desde.X == hacia.X && desde.Y == hacia.Y) return false;

            Pieza? reina = tablero.ObtenerPieza(desde);
            if (reina == null) return false;

            int deltaX = hacia.X - desde.X;
            int deltaY = hacia.Y - desde.Y;

            if (deltaX != 0 && deltaY != 0 && Math.Abs(deltaX) != Math.Abs(deltaY)) return false;

            int stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
            int stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

            int x = desde.X + stepX;
            int y = desde.Y + stepY;

            while (x != hacia.X || y != hacia.Y)
            {
                if (tablero.ObtenerPieza(new Posicion(x, y)) != null)
                    return false;
                x += stepX;
                y += stepY;
            }

            Pieza? piezaDestino = tablero.ObtenerPieza(hacia);
            return piezaDestino == null || piezaDestino.Propietario != reina.Propietario;
        }
    }

    public sealed class EstrategiaMovimientoRey : IEstrategiaMovimientoPieza
    {
        public bool EsMovimientoValido(Posicion desde, Posicion hacia, Tablero tablero)
        {
            if (!desde.EsValida() || !hacia.EsValida()) return false;
            if (desde.X == hacia.X && desde.Y == hacia.Y) return false;

            Pieza? rey = tablero.ObtenerPieza(desde);
            if (rey == null) return false;

            int deltaX = Math.Abs(hacia.X - desde.X);
            int deltaY = Math.Abs(hacia.Y - desde.Y);

            if (deltaX > 1 || deltaY > 1) return false;

            Pieza? piezaDestino = tablero.ObtenerPieza(hacia);
            return piezaDestino == null || piezaDestino.Propietario != rey.Propietario;
        }
    }

    #endregion

    #region Modelo de Dominio

    public sealed class Pieza
    {
        public TipoPieza Tipo { get; }
        public IdJugador Propietario { get; }
        public Posicion PosicionActual { get; set; }
        private readonly IEstrategiaMovimientoPieza _estrategiaMovimiento;

        public Pieza(TipoPieza tipo, IdJugador propietario, Posicion posicion, IEstrategiaMovimientoPieza estrategiaMovimiento)
        {
            Tipo = tipo;
            Propietario = propietario;
            PosicionActual = posicion;
            _estrategiaMovimiento = estrategiaMovimiento;
        }

        public bool PuedeMoverseA(Posicion destino, Tablero tablero)
        {
            return _estrategiaMovimiento.EsMovimientoValido(PosicionActual, destino, tablero);
        }
    }

    public sealed class Tablero
    {
        private readonly Pieza?[,] _matriz;
        private readonly List<Pieza> _piezasActivas;

        public Tablero()
        {
            _matriz = new Pieza[8, 8];
            _piezasActivas = new List<Pieza>(24);
            InicializarTablero();
        }

        private void InicializarTablero()
        {
            IEstrategiaMovimientoPieza estrategiaPeon = new EstrategiaMovimientoPeon();
            IEstrategiaMovimientoPieza estrategiaCaballo = new EstrategiaMovimientoCaballo();
            IEstrategiaMovimientoPieza estrategiaTorre = new EstrategiaMovimientoTorre();
            IEstrategiaMovimientoPieza estrategiaAlfil = new EstrategiaMovimientoAlfil();
            IEstrategiaMovimientoPieza estrategiaReina = new EstrategiaMovimientoReina();
            IEstrategiaMovimientoPieza estrategiaRey = new EstrategiaMovimientoRey();

            ColocarNuevaPieza(TipoPieza.Torre, IdJugador.Esmeralda, new Posicion(0, 0), estrategiaTorre);
            ColocarNuevaPieza(TipoPieza.Caballo, IdJugador.Esmeralda, new Posicion(1, 0), estrategiaCaballo);
            ColocarNuevaPieza(TipoPieza.Alfil, IdJugador.Esmeralda, new Posicion(2, 0), estrategiaAlfil);
            ColocarNuevaPieza(TipoPieza.Reina, IdJugador.Esmeralda, new Posicion(3, 0), estrategiaReina);
            ColocarNuevaPieza(TipoPieza.Rey, IdJugador.Esmeralda, new Posicion(4, 0), estrategiaRey);
            ColocarNuevaPieza(TipoPieza.Alfil, IdJugador.Esmeralda, new Posicion(5, 0), estrategiaAlfil);
            ColocarNuevaPieza(TipoPieza.Caballo, IdJugador.Esmeralda, new Posicion(6, 0), estrategiaCaballo);
            ColocarNuevaPieza(TipoPieza.Torre, IdJugador.Esmeralda, new Posicion(7, 0), estrategiaTorre);

            for (int x = 0; x < 8; x++)
            {
                ColocarNuevaPieza(TipoPieza.Peon, IdJugador.Esmeralda, new Posicion(x, 1), estrategiaPeon);
            }

            ColocarNuevaPieza(TipoPieza.Torre, IdJugador.Rubi, new Posicion(0, 7), estrategiaTorre);
            ColocarNuevaPieza(TipoPieza.Caballo, IdJugador.Rubi, new Posicion(1, 7), estrategiaCaballo);
            ColocarNuevaPieza(TipoPieza.Alfil, IdJugador.Rubi, new Posicion(2, 7), estrategiaAlfil);
            ColocarNuevaPieza(TipoPieza.Reina, IdJugador.Rubi, new Posicion(3, 7), estrategiaReina);
            ColocarNuevaPieza(TipoPieza.Rey, IdJugador.Rubi, new Posicion(4, 7), estrategiaRey);
            ColocarNuevaPieza(TipoPieza.Alfil, IdJugador.Rubi, new Posicion(5, 7), estrategiaAlfil);
            ColocarNuevaPieza(TipoPieza.Caballo, IdJugador.Rubi, new Posicion(6, 7), estrategiaCaballo);
            ColocarNuevaPieza(TipoPieza.Torre, IdJugador.Rubi, new Posicion(7, 7), estrategiaTorre);

            for (int x = 0; x < 8; x++)
            {
                ColocarNuevaPieza(TipoPieza.Peon, IdJugador.Rubi, new Posicion(x, 6), estrategiaPeon);
            }
        }

        private void ColocarNuevaPieza(TipoPieza tipo, IdJugador propietario, Posicion pos, IEstrategiaMovimientoPieza estrategia)
        {
            var pieza = new Pieza(tipo, propietario, pos, estrategia);
            _matriz[pos.X, pos.Y] = pieza;
            _piezasActivas.Add(pieza);
        }

        public Pieza? ObtenerPieza(Posicion pos)
        {
            if (!pos.EsValida()) return null;
            return _matriz[pos.X, pos.Y];
        }

        public void EjecutarMovimiento(Posicion desde, Posicion hacia, out Pieza? piezaCapturada)
        {
            piezaCapturada = _matriz[hacia.X, hacia.Y];
            Pieza? piezaEnMovimiento = _matriz[desde.X, desde.Y];

            if (piezaEnMovimiento == null)
                throw new InvalidOperationException("No existe ninguna pieza en la coordenada de origen.");

            if (piezaCapturada != null)
            {
                _piezasActivas.Remove(piezaCapturada);
            }

            // Actualizar la matriz de escaques y el estado interno de la pieza
            _matriz[desde.X, desde.Y] = null;
            _matriz[hacia.X, hacia.Y] = piezaEnMovimiento;
            piezaEnMovimiento.PosicionActual = hacia;
        }

        public int ObtenerCantidadActivas(IdJugador jugador)
        {
            int contador = 0;
            for (int i = 0; i < _piezasActivas.Count; i++)
            {
                if (_piezasActivas[i].Propietario == jugador) contador++;
            }
            return contador;
        }
    }

    #endregion

    #region Motor del Juego (Game Engine)

    public sealed class MotorJuego
    {
        public Tablero Tablero { get; private set; }
        public IdJugador TurnoActual { get; private set; }
        public bool JuegoTerminado { get; private set; }
        public IdJugador? Ganador { get; private set; }

        private readonly List<RegistroMovimiento> _historialMovimientos;
        private readonly List<TipoPieza> _cementerioEsmeralda;
        private readonly List<TipoPieza> _cementerioRubi;

        public IReadOnlyList<RegistroMovimiento> HistorialMovimientos => _historialMovimientos;
        public IReadOnlyList<TipoPieza> CementerioEsmeralda => _cementerioEsmeralda;
        public IReadOnlyList<TipoPieza> CementerioRubi => _cementerioRubi;

        public string NombreEsmeralda { get; set; } = "Esmeralda (J1)";
        public string NombreRubi { get; set; } = "Rubí (J2)";

        public MotorJuego()
        {
            Tablero = new Tablero();
            TurnoActual = IdJugador.Esmeralda;
            JuegoTerminado = false;
            Ganador = null;
            _historialMovimientos = new List<RegistroMovimiento>(128);
            _cementerioEsmeralda = new List<TipoPieza>(10);
            _cementerioRubi = new List<TipoPieza>(10);
        }

        public bool ProcesarTurno(Posicion desde, Posicion hacia, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (JuegoTerminado)
            {
                mensajeError = "El juego ya ha finalizado.";
                return false;
            }

            if (!desde.EsValida() || !hacia.EsValida())
            {
                mensajeError = "Las coordenadas ingresadas exceden los límites del tablero.";
                return false;
            }

            Pieza? pieza = Tablero.ObtenerPieza(desde);
            if (pieza == null)
            {
                mensajeError = "La casilla de origen seleccionada está vacía.";
                return false;
            }

            if (pieza.Propietario != TurnoActual)
            {
                string nombreActivo = (TurnoActual == IdJugador.Esmeralda) ? NombreEsmeralda : NombreRubi;
                mensajeError = $"No puedes mover esa pieza. Es el turno de: {nombreActivo}.";
                return false;
            }

            if (!pieza.PuedeMoverseA(hacia, Tablero))
            {
                mensajeError = "Movimiento inválido para el tipo de pieza seleccionado según las reglas.";
                return false;
            }

            // Ejecución segura del movimiento en la matriz
            Tablero.EjecutarMovimiento(desde, hacia, out Pieza? capturada);

            bool esCaptura = capturada != null;
            TipoPieza? tipoCapturado = capturada?.Tipo;

            if (capturada != null)
            {
                if (capturada.Propietario == IdJugador.Esmeralda)
                    _cementerioRubi.Add(capturada.Tipo); // Capturada por el jugador Rubí
                else
                    _cementerioEsmeralda.Add(capturada.Tipo);
            }

            _historialMovimientos.Add(new RegistroMovimiento(TurnoActual, pieza.Tipo, desde, hacia, esCaptura, tipoCapturado));

            ComprobarCondicionesVictoria();

            if (!JuegoTerminado)
            {
                TurnoActual = (TurnoActual == IdJugador.Esmeralda) ? IdJugador.Rubi : IdJugador.Esmeralda;
            }

            return true;
        }

        private void ComprobarCondicionesVictoria()
        {
            int conteoEsmeralda = Tablero.ObtenerCantidadActivas(IdJugador.Esmeralda);
            int conteoRubi = Tablero.ObtenerCantidadActivas(IdJugador.Rubi);

            if (conteoEsmeralda == 0)
            {
                JuegoTerminado = true;
                Ganador = IdJugador.Rubi;
            }
            else if (conteoRubi == 0)
            {
                JuegoTerminado = true;
                Ganador = IdJugador.Esmeralda;
            }
        }
    }

    #endregion

    #region Capa de Presentación (Interfaz de Consola)

    public sealed class InterfazConsola
    {
        private readonly MotorJuego _motor;
        private string _mensajeEstado = "¡Bienvenidos al juego de estrategia! Ingrese un comando válido.";
        private ConsoleColor _colorEstado = ConsoleColor.Cyan;

        public InterfazConsola(MotorJuego motor)
        {
            _motor = motor;
        }

        public int EjecutarPartida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.WriteLine("                      CONFIGURACIÓN DE LA PARTIDA                     ");
            Console.WriteLine("=====================================================================");
            Console.ResetColor();

            Console.Write("\nNombre del Jugador 1 (TROPAS ESMERALDA - Inicia Abajo): ");
            string n1 = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(n1)) _motor.NombreEsmeralda = n1;

            Console.Write("Nombre del Jugador 2 (TROPAS RUBÍ - Inicia Arriba): ");
            string n2 = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(n2)) _motor.NombreRubi = n2;

            while (!_motor.JuegoTerminado)
            {
                RenderizarVista();
                ProcesarEntradaUsuario();
            }

            return RenderizarFinJuego();
        }

        private void RenderizarVista()
        {
            Console.Clear();
            DibujarEncabezado();
            DibujarTablero(out int filaInferiorTablero);
            DibujarHUD(filaInferiorTablero);
            DibujarBarraEstado();
        }

        private void DibujarEncabezado()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.Write("        CAMPO DE BATALLA: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(_motor.NombreEsmeralda.ToUpper());
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" VS ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(_motor.NombreRubi.ToUpper());
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.ResetColor();
        }

        private void DibujarTablero(out int filaInferiorTablero)
        {
            Console.WriteLine();
            for (int y = 7; y >= 0; y--)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" {y + 1}  ");
                Console.ResetColor();

                for (int x = 0; x < 8; x++)
                {
                    bool esCasillaOscura = (x + y) % 2 == 0;
                    Console.BackgroundColor = esCasillaOscura ? ConsoleColor.DarkGray : ConsoleColor.Gray;

                    Pieza? pieza = _motor.Tablero.ObtenerPieza(new Posicion(x, y));
                    if (pieza != null)
                    {
                        Console.ForegroundColor = (pieza.Propietario == IdJugador.Esmeralda) ? ConsoleColor.Green : ConsoleColor.Red;
                        string glifo = pieza.Tipo switch
                        {
                            TipoPieza.Peon => " ♙ ",
                            TipoPieza.Caballo => " ♞ ",
                            TipoPieza.Torre => " ♜ ",
                            TipoPieza.Alfil => " ♝ ",
                            TipoPieza.Reina => " ♛ ",
                            TipoPieza.Rey => " ♚ ",
                            _ => " ? "
                        };
                        Console.Write(glifo);
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("     A  B  C  D  E  F  G  H ");
            Console.ResetColor();
            Console.WriteLine();

            filaInferiorTablero = Console.CursorTop;
        }

        private void DibujarHUD(int filaInferiorTablero)
        {
            Console.Write("TURNO ACTUAL: ");
            if (_motor.TurnoActual == IdJugador.Esmeralda)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{_motor.NombreEsmeralda} (Esmeralda)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{_motor.NombreRubi} (Rubí)");
            }
            Console.ResetColor();
            Console.WriteLine("---------------------------------------------------------------------");

            Console.Write($"Bajas infligidas por {_motor.NombreEsmeralda}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            RenderizarCementerio(_motor.CementerioEsmeralda);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Bajas infligidas por {_motor.NombreRubi}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            RenderizarCementerio(_motor.CementerioRubi);
            Console.ResetColor();

            Console.WriteLine("---------------------------------------------------------------------");

            Console.WriteLine("ÚLTIMOS MOVIMIENTOS:");
            int cantidadHistorial = _motor.HistorialMovimientos.Count;
            int indiceInicio = Math.Max(0, cantidadHistorial - 3);

            for (int i = indiceInicio; i < cantidadHistorial; i++)
            {
                var registro = _motor.HistorialMovimientos[i];
                string desdeStr = $"{(char)('A' + registro.Desde.X)}{registro.Desde.Y + 1}";
                string haciaStr = $"{(char)('A' + registro.Hacia.X)}{registro.Hacia.Y + 1}";
                string nombrePieza = ObtenerNombrePiezaEspanol(registro.TipoDePieza);
                string equipo = registro.Jugador == IdJugador.Esmeralda ? _motor.NombreEsmeralda : _motor.NombreRubi;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" [{i + 1}] ");
                Console.ForegroundColor = registro.Jugador == IdJugador.Esmeralda ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"{equipo} ({nombrePieza})");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" se movió de {desdeStr} a {haciaStr}");

                if (registro.EsCaptura)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($" ¡Capturando {ObtenerNombrePiezaEspanol(registro.TipoCapturado)}!");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.WriteLine("=====================================================================");
        }

        private void RenderizarCementerio(IReadOnlyList<TipoPieza> cementerio)
        {
            if (cementerio.Count == 0)
            {
                Console.WriteLine("[Ninguna]");
                return;
            }

            for (int i = 0; i < cementerio.Count; i++)
            {
                string glifo = cementerio[i] switch
                {
                    TipoPieza.Peon => "♟",
                    TipoPieza.Caballo => "♞",
                    TipoPieza.Torre => "♜",
                    TipoPieza.Alfil => "♝",
                    TipoPieza.Reina => "♛",
                    TipoPieza.Rey => "♚",
                    _ => "?"
                };
                Console.Write(glifo + " ");
            }
            Console.WriteLine();
        }

        private void DibujarBarraEstado()
        {
            Console.WriteLine();
            Console.ForegroundColor = _colorEstado;
            Console.WriteLine($"LOG STATUS: {_mensajeEstado}");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Ingrese comando de movimiento (Ej: B2 B3): ");
        }

        private void ProcessInput()
        {
            ProcesarEntradaUsuario();
        }

        private void ProcesarEntradaUsuario()
        {
            string entrada = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(entrada))
            {
                AsignarEstado("La entrada no puede estar vacía.", ConsoleColor.Yellow);
                return;
            }

            string[] tokens = entrada.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                AsignarEstado("Formato incorrecto. Ingrese origen y destino (Ej: B2 B3).", ConsoleColor.Yellow);
                return;
            }

            if (!ConvertirCoordenada(tokens[0], out Posicion desde) || !ConvertirCoordenada(tokens[1], out Posicion hacia))
            {
                AsignarEstado("Coordenadas inválidas. Use letras de A-H y números del 1-8 (Ej: A2).", ConsoleColor.Yellow);
                return;
            }

            if (_motor.ProcesarTurno(desde, hacia, out string error))
            {
                AsignarEstado("Movimiento ejecutado correctamente.", ConsoleColor.Cyan);
            }
            else
            {
                AsignarEstado(error, ConsoleColor.Yellow);
            }
        }

        private bool ConvertirCoordenada(string token, out Posicion posicion)
        {
            posicion = new Posicion(-1, -1);
            if (token.Length != 2) return false;

            char caracterColumna = token[0];
            char caracterFila = token[1];

            int x = caracterColumna - 'A';
            int y = caracterFila - '1';

            var posTemporal = new Posicion(x, y);
            if (posTemporal.EsValida())
            {
                posicion = posTemporal;
                return true;
            }

            return false;
        }

        private void AsignarEstado(string msg, ConsoleColor color)
        {
            _mensajeEstado = msg;
            _colorEstado = color;
        }

        private string ObtenerNombrePiezaEspanol(TipoPieza? tipo)
        {
            if (tipo == null) return string.Empty;
            return tipo switch
            {
                TipoPieza.Peon => "Peón",
                TipoPieza.Caballo => "Caballo",
                TipoPieza.Torre => "Torre",
                TipoPieza.Alfil => "Alfil",
                TipoPieza.Reina => "Reina",
                TipoPieza.Rey => "Rey",
                _ => "Desconocida"
            };
        }

        private int RenderizarFinJuego()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=====================================================================");
            Console.WriteLine("                        ¡FIN DE LA PARTIDA!                          ");
            Console.WriteLine("=====================================================================");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  EL GANADOR ABSOLUTO ES EL ");

            string nombreGanador = "";
            int piezasCapturadas = 0;

            if (_motor.Ganador == IdJugador.Esmeralda)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                nombreGanador = _motor.NombreEsmeralda;
                piezasCapturadas = _motor.CementerioEsmeralda.Count;
                Console.WriteLine($"{nombreGanador.ToUpper()} (TROPAS ESMERALDA)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                nombreGanador = _motor.NombreRubi;
                piezasCapturadas = _motor.CementerioRubi.Count;
                Console.WriteLine($"{nombreGanador.ToUpper()} (TROPAS RUBÍ)");
            }

            // Cálculo obligatorio exacto de la propuesta: +10 puntos por pieza eliminada
            int puntajeFinal = piezasCapturadas * 10;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nPuntaje Obtenido: {puntajeFinal} puntos (Bajas enemigas: {piezasCapturadas}).");
            Console.WriteLine("=====================================================================");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Presione cualquier tecla para regresar al Menú Principal...");
            Console.ReadKey();

            return puntajeFinal;
        }
    }

    #endregion

    #region Sistema de Gestión del Menú, Récords y Login Global

    public sealed class SistemaPrincipal
    {
        private string _recordJugador = "";
        private int _recordPuntaje = -1; 

        public void Ejecutar()
        {
            ControlarLogin();
            MostrarMenuPrincipal();
        }

        private void ControlarLogin()
        {
            string usuarioDefinido = "grupo3";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.WriteLine("                SISTEMA DE CONTROL DE ACCESO SEGURO                  ");
            Console.WriteLine("=====================================================================");
            Console.ResetColor();

            while (true)
            {
                Console.Write("Ingrese Usuario del Grupo: ");
                string usuarioIngresado = Console.ReadLine();

                Console.Write("Ingrese Contraseña del Sistema: ");
                string passIngresada = LeerContrasenaOculta();

                if (usuarioIngresado == usuarioDefinido && ValidarEstructuraContrasena(passIngresada))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n\n[Éxito] Autenticación correcta. Inicializando Entorno...");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1200);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\n[Error] Credenciales incorrectas o la estructura de la contraseña");
                    Console.WriteLine("no cumple con las políticas de caracteres especiales exigidas.");
                    Console.WriteLine("Intente de nuevo.\n");
                    Console.ResetColor();
                }
            }
        }

        private string LeerContrasenaOculta()
        {
            string pass = "";
            ConsoleKeyInfo llave;
            do
            {
                llave = Console.ReadKey(true);
                if (llave.Key != ConsoleKey.Backspace && llave.Key != ConsoleKey.Enter)
                {
                    pass += llave.KeyChar;
                    Console.Write("*");
                }
                else if (llave.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, (pass.Length - 1));
                    Console.Write("\b \b");
                }
            } while (llave.Key != ConsoleKey.Enter);
            return pass;
        }

        private bool ValidarEstructuraContrasena(string pass)
        {
            if (pass.Length < 8) return false;

            bool tieneMayuscula = false;
            bool tieneMinuscula = false;
            bool tieneNumero = false;
            bool tieneEspecial = false;
            string caracteresEspeciales = "!@#$%^&*()_+-[{]};:<>|./?,=";

            foreach (char c in pass)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                else if (char.IsLower(c)) tieneMinuscula = true;
                else if (char.IsDigit(c)) tieneNumero = true;
                else if (caracteresEspeciales.Contains(c.ToString())) tieneEspecial = true;
            }

            return tieneMayuscula && tieneMinuscula && tieneNumero && tieneEspecial;
        }

        private void MostrarMenuPrincipal()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("=====================================================================");
                Console.WriteLine("                    MENÚ PRINCIPAL - STRATEGIC GAME                  ");
                Console.WriteLine("=====================================================================");
                Console.ResetColor();
                Console.WriteLine("  1. Iniciar Nueva Partida Escaqueada");
                Console.WriteLine("  2. Ver Reglas de Movimiento y Combate");
                Console.WriteLine("  3. Consultar Salón de la Fama (Puntaje Más Alto)");
                Console.WriteLine("  4. Cerrar Sistema");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("=====================================================================");
                Console.ResetColor();
                Console.Write("Seleccione una opción de ejecución: ");

                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            MotorJuego motor = new MotorJuego();
                            InterfazConsola ui = new InterfazConsola(motor);

                            int puntajePartida = ui.EjecutarPartida();

                            string ganadorReal = (motor.Ganador == IdJugador.Esmeralda) ? motor.NombreEsmeralda : motor.NombreRubi;
                            ActualizarRecordGlobal(ganadorReal, puntajePartida);
                            break;
                        case 2:
                            MostrarReglasEstrategia();
                            break;
                        case 3:
                            MostrarRecordGlobal();
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\nFinalizando procesos seguros del sistema... Gracias por jugar.");
                            Console.ResetColor();
                            break;
                        default:
                            Console.WriteLine("\n[Opción Inválida] Ingrese un valor numérico entre 1 and 4.");
                            EsperarTecla();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\n[Error] Por favor, introduzca una opción de menú válida.");
                    EsperarTecla();
                }
            } while (opcion != 4);
        }

        private void MostrarReglasEstrategia()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.WriteLine("                     REGLAMENTO OFICIAL DE COMBATE                   ");
            Console.WriteLine("=====================================================================");
            Console.ResetColor();
            Console.WriteLine("\nMOVIMIENTOS DE LAS PIEZAS:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  • PEÓN (♟): ");
            Console.ResetColor();
            Console.WriteLine("Se desplaza estrictamente 1 casilla hacia adelante.");
            Console.WriteLine("              Captura piezas enemigas únicamente avanzando 1 casilla en diagonal.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n  • CABALLO (♞): ");   
            Console.ResetColor();
            Console.WriteLine("Se mueve en forma de 'L' (2 casillas en una dirección y 1 perpendicular).");
            Console.WriteLine("                 Es la única pieza capaz de saltar sobre otras unidades.");

            Console.WriteLine("\nCONDICIÓN DE VICTORIA:");
            Console.WriteLine("  - Eliminar por completo todas las piezas del ejército oponente del tablero.");

            Console.WriteLine("\nSISTEMA DE PUNTUACIÓN:");
            Console.WriteLine("  - Cada pieza enemiga eliminada y enviada al cementerio otorga +10 puntos.");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.ResetColor();
            EsperarTecla();
        }

        private void ActualizarRecordGlobal(string nombre, int puntaje)
        {
            if (_recordPuntaje == -1 || puntaje > _recordPuntaje)
            {
                _recordPuntaje = puntaje;
                _recordJugador = nombre;
            }
        }

        private void MostrarRecordGlobal()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====================================================================");
            Console.WriteLine("                 SALÓN DE LA FAMA - RECORD HISTÓRICO                ");
            Console.WriteLine("=====================================================================");
            Console.ResetColor();

            if (_recordPuntaje == -1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n      Aún no hay puntajes registrados");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Soberano Invicto:  {_recordJugador}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   Puntaje Máximo:    {_recordPuntaje} Puntos");
                Console.ResetColor();
                Console.WriteLine("\n   Fórmula: (Piezas Enemigas Capturadas × 10 Puntos)");
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n=====================================================================");
            Console.ResetColor();
            EsperarTecla();
        }

        private void EsperarTecla()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    #endregion
}