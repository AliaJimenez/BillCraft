using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;

#region Producto
public class Producto
{
    public string Codigo { get; private set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }

    public Producto (string nombre, decimal precio, int numeroSecuencial)
    {
        Codigo = $"P-{numeroSecuencial:D3}";//obtener un codigo unico y permanente 
        Nombre = nombre;
        Precio = precio;
    }

    public Producto (string codigo, string nombre, decimal precio)
    {
        Codigo = codigo;
        Nombre = nombre;
        Precio = precio;
    }
}
#endregion

#region DetalleFactura
public class DetalleFactura
{
    public Producto Producto { get; private set; }
    public int Cantidad { get; private set; }

    public decimal SubTotalProducto
    {
        get
        {
            return Cantidad * Producto.Precio;
        }
    }

    public DetalleFactura(Producto producto, int cantidad)
    {
        Producto = producto; 
        Cantidad = cantidad;
    }
}
#endregion

#region Factura
public class Factura //representa una factura real de un negocio
{
    public string Numero { get; private set; } 
    public DateTime Fecha { get; private set; }
    public List<DetalleFactura> Detalles { get; private set; } //guarda todos los productos vendidos
    public const decimal TasaImpuesto = 0.16m;

    public decimal SubTotal
    {
        get
        {
            return Detalles.Sum(d => d.SubTotalProducto); //suma el SubTotalProducto de cada detalle
        }
    }

    public decimal Impuestos
    {
        get
        {
            return SubTotal * TasaImpuesto;
        }
    }

    public decimal Total
    {
        get
        {
            return SubTotal + Impuestos;
        }
    }

    // Constructor para crear nuevas facturas
    public Factura(int numeroSecuencial)
    {
        Numero = $"F-{numeroSecuencial.ToString("D4")}"; 
        Fecha = DateTime.Now;
        Detalles = new List<DetalleFactura>();
    }

    // Constructor para cargar facturas existentes desde archivo
    public Factura(string numero, DateTime fecha)
    {
        Numero = numero;
        Fecha = fecha;
        Detalles = new List<DetalleFactura>();
    }

    public void AgregarDetalle(DetalleFactura detalle) //agrega un producto a la factura.
    {
        Detalles.Add(detalle);
    }
}
#endregion

class Program
{
    #region Percistencia
        static void GuardarProductos(List<Producto> productos)
        {
            List<string> lineas = new List<string>();
            lineas.Add("codigo|nombre|precio");

            foreach (Producto p in productos)
            {
                string linea = $"{p.Codigo}|{p.Nombre}|{p.Precio}";
                lineas.Add(linea);
            }

            File.WriteAllLines("productos.txt", lineas);

            Console.WriteLine("Productos guardados correctamente en el archivo.");
        }

        static List<Producto> CargarProductos()
        {
            List<Producto> productos = new List<Producto>();

            if (!File.Exists("productos.txt"))
            {
                Console.WriteLine("Archivo no encontrado");
                return productos;
            }

            string[] lineas = File.ReadAllLines("productos.txt");

            for (int i = 1; i < lineas.Length; i++)
            {
                string linea = lineas[i];
                string[] partes = linea.Split('|');
                
                string codigo = partes[0];
                string nombre = partes[1];
                decimal precio = Convert.ToDecimal(partes[2]);

                Producto producto = new Producto(codigo, nombre, precio);
                productos.Add(producto);
            };

            Console.WriteLine($"{productos.Count} productos cargados desde el archivo.");
            return productos;

        }

        static void GuardarFacturas(List<Factura> facturas)
        {
            List<string> lineas = new List<string>();
            
            // Encabezado
            lineas.Add("numero_factura|fecha|codigo_producto|cantidad|nombre_producto|precio_producto");
            
            // Convertir cada factura (y sus detalles) a líneas
            foreach (Factura factura in facturas)
            {
                foreach (DetalleFactura detalle in factura.Detalles)
                {
                    string linea = $"{factura.Numero}|{factura.Fecha}|{detalle.Producto.Codigo}|{detalle.Cantidad}|{detalle.Producto.Nombre}|{detalle.Producto.Precio}";
                    lineas.Add(linea);
                }
            }
            
            File.WriteAllLines("facturas.txt", lineas);
            Console.WriteLine("Facturas guardadas en el archivo");
        }
        static List<Factura> CargarFacturas(List<Producto> productos)
        {
            List<Factura> facturas = new List<Factura>();

            if (!File.Exists("facturas.txt"))
            {
                Console.WriteLine("Archivo no encontrado");
                return facturas;
            }

            string[] lineas = File.ReadAllLines("facturas.txt");
            
            //Diccionario para agrupar detalles por número de factura
            Dictionary<string, List<string[]>> facturasPorNumero = new Dictionary<string, List<string[]>>();
            
            // Saltar encabezado y agrupar líneas por número de factura
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] partes = lineas[i].Split('|');
                string numeroFactura = partes[0];
                
                // Si esta factura no existe en el diccionario, crearla
                if (!facturasPorNumero.ContainsKey(numeroFactura))
                {
                    facturasPorNumero[numeroFactura] = new List<string[]>();
                }
                
                // Agregar esta línea al grupo de esa factura
                facturasPorNumero[numeroFactura].Add(partes);
            }
            
            // Ahora construir cada factura
            foreach (var grupo in facturasPorNumero)
            {
                string numeroFactura = grupo.Key;
                List<string[]> detallesLineas = grupo.Value;
                
                // Crear la factura (necesitamos un constructor especial)
                // Tomaremos la fecha de la primera línea (todas tienen la misma)
                DateTime fecha = DateTime.Parse(detallesLineas[0][1]);
                
                Factura factura = new Factura(numeroFactura, fecha);
                
                // Agregar cada detalle
                foreach (string[] partes in detallesLineas)
                {
                    string codigoProducto = partes[2];
                    int cantidad = int.Parse(partes[3]);
                    string nombreProducto = partes[4];
                    decimal precioProducto = decimal.Parse(partes[5]);
                    
                    // Reconstruir el producto
                    Producto producto = new Producto(codigoProducto, nombreProducto, precioProducto);
                    
                    // Crear el detalle
                    DetalleFactura detalle = new DetalleFactura(producto, cantidad);
                    
                    // Agregar a la factura
                    factura.AgregarDetalle(detalle);
                }
                
                facturas.Add(factura);
            }
            
            Console.WriteLine($"{facturas.Count} facturas cargadas desde el archivo");
            return facturas;
        }

    #endregion

    #region Menu de uso del usuario
    static void Main(string[] args)
    {
        List<Producto> productos = CargarProductos();
        List<Factura> facturas = CargarFacturas(productos);
        string opcion = "";
        string SegOpcion = "";

        do
        {
            Console.Clear();
            Console.WriteLine("\n⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍ MENÚ ⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
            Console.WriteLine(" 1. Gestión de productos");
            Console.WriteLine(" 2. Nueva venta");
            Console.WriteLine(" 3. Ver facturas registradas");
            Console.WriteLine(" 4. Salir");
            Console.WriteLine("⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
            Console.Write("Seleccione una opción: ");

            opcion = Console.ReadLine()!;

            switch (opcion)
            {
                case "1":
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("\n⚍⚍⚍⚍⚍⚍ GESTIÓN DE PRODUCTOS ⚍⚍⚍⚍⚍⚍⚍");
                        Console.WriteLine(" 1. Agregar nuevo producto");
                        Console.WriteLine(" 2. Ver productos diponibles");
                        Console.WriteLine(" 3. Salir");
                        Console.WriteLine("⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                        Console.Write("Seleccione una opción: ");

                        SegOpcion = Console.ReadLine()!;

                        if (SegOpcion == "1")
                        {
                            Console.Clear();
                            bool agregarProductos = true;

                            while(agregarProductos)
                            {
                                Console.WriteLine("\n⚍⚍⚍⚍⚍⚍ AGREGAR NUEVO PRODUCTO ⚍⚍⚍⚍⚍⚍");
                                
                                Console.Write("Nombre del producto: ");
                                string nombre = Console.ReadLine()!;
                                
                                while (string.IsNullOrWhiteSpace(nombre))
                                {
                                    Console.WriteLine("El nombre no puede estar vacío.");
                                    Console.Write("Nombre del producto: ");
                                    nombre = Console.ReadLine()!;
                                }
                                
                                decimal precio = 0;
                                bool precioValido = false;
                                
                                while (!precioValido)
                                {
                                    Console.Write("Precio del producto: ");
                                    string inputPrecio = Console.ReadLine()!;
                                    
                                    if (decimal.TryParse(inputPrecio, out precio) && precio > 0)
                                    {
                                        precioValido = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Precio inválido. Debe ser un número mayor a 0.");
                                    }
                                }
                                
                                int numeroSecuencial = productos.Count + 1;
                                // Crear el producto
                                Producto nuevoProducto = new Producto(nombre, precio, numeroSecuencial);
                                
                                // Agregarlo a la lista
                                productos.Add(nuevoProducto);
                                GuardarProductos(productos);
                                
                                Console.WriteLine($"\nProducto agregado exitosamente!");
                                Console.WriteLine($"  Código: {nuevoProducto.Codigo}");
                                Console.WriteLine($"  Nombre: {nuevoProducto.Nombre}");
                                Console.WriteLine($"  Precio: ${nuevoProducto.Precio:F2}");

                                string resp = "";
                                while(resp != "s" && resp != "n")
                                {
                                    Console.Write("\n¿Desea agregar otro producto? (s/n): ");
                                    resp = Console.ReadLine()!.ToLower();

                                    if(resp != "s" && resp != "n")
                                    {
                                        Console.WriteLine("\nIngrese solamente S o N");
                                    }
                                }
                                
                                if (resp != "s" && resp != "si" && resp != "sí")
                                {
                                    agregarProductos = false;
                                }    
                            }

                        }

                        else if (SegOpcion == "2")
                        {
                            Console.Clear();
                            Console.WriteLine("\n⚍⚍⚍⚍⚍PRODUCTOS DISPONIBLES⚍⚍⚍⚍⚍");
    
                            if (productos.Count == 0)
                            {
                                Console.WriteLine("No hay productos registrados");
                            }
                            else
                            {
                                Console.WriteLine($"\nTotal de productos: {productos.Count}\n");
                                
                                foreach (Producto producto in productos)
                                {
                                    
                                    Console.WriteLine("⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                                    Console.WriteLine($"Código: {producto.Codigo}");
                                    Console.WriteLine($"Nombre: {producto.Nombre}");
                                    Console.WriteLine($"Precio: ${producto.Precio:F2}");
                                }
                            }

                            Console.Write("Presione cualquier tecla para contituar...");
                            Console.ReadKey();

                        }

                        else if (SegOpcion == "3")
                        {
                            Thread.Sleep(300);
                            Console.WriteLine("\nVolviendo al menú principal...");
                            break;
                        }

                        else
                        {
                            Console.WriteLine("\nElije una opción valida.");
                        }
                    }while (SegOpcion != "3");
                    Console.Write("Presione cualquier tecla para contituar...");
                    Console.ReadKey();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("\n⚍⚍⚍⚍⚍⚍⚍⚍⚍NUEVA VENTA⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                    
                    if (productos.Count == 0)
                    {
                        Console.WriteLine("No hay productos registrados. Primero debes agregar productos.");
                        break; // Sale del case y vuelve al menú principal
                    }
                    
                    int numeroSecuencialFactura = facturas.Count + 1;
                    Factura nuevaFactura = new Factura(numeroSecuencialFactura);

                    Console.Clear();
                    bool agregarMasProductos = true;
                    
                    while (agregarMasProductos)
                    {
                        Console.WriteLine("\n⚍⚍⚍⚍⚍⚍ PRODUCTOS DISPONIBLES ⚍⚍⚍⚍⚍⚍");
                        
                        // Mostrar productos con ÍNDICE (1, 2, 3...)
                        for (int i = 0; i < productos.Count; i++)
                        {
                            Producto p = productos[i];
                            Console.WriteLine($"{i + 1}. {p.Codigo}: {p.Nombre} - ${p.Precio:F2}");
                        }
                        
                        Console.Write("\nSeleccione el número del producto (0 para cancelar): ");
                        string inputIndice = Console.ReadLine()!;
                        
                        // Convertir el input a número
                        if (!int.TryParse(inputIndice, out int indice))
                        {
                            Console.WriteLine("Opción inválida.");
                            continue;
                        }
                        
                        // Si el usuario pone 0, cancela
                        if (indice == 0)
                        {
                            Console.WriteLine("Venta cancelada.");
                            agregarMasProductos = false;
                            continue;
                        }
                        
                        /* Validar que el índice esté en el rango válido
                           El usuario escribe 1, 2, 3... pero en el array es 0, 1, 2... */
                        if (indice < 1 || indice > productos.Count)
                        {
                            Console.WriteLine($"Opción inválida. Debe estar entre 1 y {productos.Count}.");
                            continue;
                        }
                        
                        // Obtener el producto seleccionado (restar 1 porque el array empieza en 0)
                        Producto productoSeleccionado = productos[indice - 1];
                        
                        // Pedir cantidad
                        Console.Write($"¿Cuántas unidades de '{productoSeleccionado.Nombre}'? ");
                        string inputCantidad = Console.ReadLine()!;
                        
                        if (!int.TryParse(inputCantidad, out int cantidad) || cantidad <= 0)
                        {
                            Console.WriteLine("Cantidad inválida. Debe ser un número mayor a 0.");
                            continue;
                        }
                        
                        // Crear el detalle y agregarlo a la factura
                        DetalleFactura detalle = new DetalleFactura(productoSeleccionado, cantidad);
                        nuevaFactura.AgregarDetalle(detalle);
                        
                        Console.WriteLine($"Agregado: {productoSeleccionado.Nombre} x {cantidad} = ${detalle.SubTotalProducto:F2}");
                        
                        string respuesta = "";

                        while(respuesta != "s" && respuesta != "n")
                        {
                            Console.Write("\n¿Desea agregar otro producto? (s/n): ");
                            respuesta = Console.ReadLine()!.ToLower();

                            if(respuesta != "s" && respuesta != "n")
                            {
                                Console.WriteLine("\nIngrese solamente S o N");
                            }
                        }
                        
                        if (respuesta != "s" && respuesta != "si" && respuesta != "sí")
                        {
                            agregarMasProductos = false;
                        }
                    }
                    
                    // Si la factura tiene al menos un producto, mostrar resumen y guardarla
                    if (nuevaFactura.Detalles.Count > 0)
                    {
                        Console.WriteLine("\n⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍ FACTURA ⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                        Console.WriteLine($"FACTURA #{nuevaFactura.Numero}");
                        Console.WriteLine($"Fecha: {nuevaFactura.Fecha:dd/MM/yyyy}");
                        Console.WriteLine("------------------------------");
                        
                        // Mostrar cada producto de forma compacta
                        foreach (DetalleFactura detalle in nuevaFactura.Detalles)
                        {
                            Console.WriteLine($"{detalle.Producto.Codigo}: {detalle.Producto.Nombre} x{detalle.Cantidad} = ${detalle.SubTotalProducto:F2}");
                        }
                        
                        Console.WriteLine("------------------------------");
                        Console.WriteLine($"Subtotal: ${nuevaFactura.SubTotal:F2}");
                        Console.WriteLine($"Impuestos (16%): ${nuevaFactura.Impuestos:F2}");
                        Console.WriteLine($"TOTAL: ${nuevaFactura.Total:F2}");
                        Console.WriteLine("⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                        
                        // Guardar la factura en la lista
                        facturas.Add(nuevaFactura);
                        GuardarFacturas(facturas);
                        Console.WriteLine("\nFactura guardada exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("\nNo se agregaron productos. Factura cancelada.");
                    }

                    Console.Write("Presione cualquier tecla para contituar...");
                    Console.ReadKey();
                    
                    break;
                
                case "3":
                    Console.WriteLine("\n⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍ FACTURAS REGISTRADAS ⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");

                    if (facturas.Count == 0)
                    {
                        Console.WriteLine("No hay facturas registradas. Volviendo al menú...");
                        break;
                    }

                    Console.WriteLine($"Total de facturas: {facturas.Count}\n");

                    foreach (Factura factura in facturas)
                    {
                        Console.WriteLine("\n⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍⚍");
                        Console.WriteLine($"Número de factura: {factura.Numero}");
                        Console.WriteLine($"Fecha: {factura.Fecha}");
                        Console.WriteLine("\nProductos:");
                        
                        foreach (DetalleFactura detalle in factura.Detalles)
                        {
                            Console.WriteLine($"  - {detalle.Cantidad} x {detalle.Producto.Nombre} @ ${detalle.Producto.Precio:F2} = ${detalle.SubTotalProducto:F2}");
                        }
                        
                        Console.WriteLine($"\nSubtotal: ${factura.SubTotal:F2}");
                        Console.WriteLine($"Impuestos (16%): ${factura.Impuestos:F2}");
                        Console.WriteLine($"TOTAL: ${factura.Total:F2}");
                    }
                    Console.Write("Presione cualquier tecla para contituar...");
                    Console.ReadKey();
                    break;

                case "4":
                    Console.WriteLine("\n¡Hasta la próxima!");
                    Console.WriteLine("Saliendo del programa...");
                    Environment.Exit(0);
                    break;
                
                default: // Este caso maneja opciones no válidas
                    Console.WriteLine("\nElije una opción valida.");
                    break;
            }
        } while (opcion != "4");
    }
    #endregion
}