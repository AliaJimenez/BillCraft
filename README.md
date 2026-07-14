# 🧾 BillCraft

> Una solución ágil, ligera y eficiente desde la consola para la gestión de inventarios y emisión de facturas comerciales. Desarrollado en **C# (.NET 9)** con procesamiento directo de flujos de texto local.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET%209.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![File IO](https://img.shields.io/badge/Persistence-Text%20Files-orange?style=for-the-badge&logo=gitbook&logoColor=white)

---

## ✨ Funcionalidades Clave

*   📦 **Control de Inventario:** Lectura y actualización dinámica del catálogo de artículos a través de un canal plano (`productos.txt`).
*   🧾 **Generación de Facturas:** Procesamiento de compras, cálculo automático de importes (e impuestos si aplica) y almacenamiento estructurado de transacciones (`facturas.txt`).
*   📈 **Validación de Datos:** Control riguroso de entradas para evitar errores de tipo o desbordamiento durante la interacción en consola.
*   ⚡ **Cero Dependencias:** Ejecución ultra rápida gracias a su persistencia en archivos locales sin necesidad de configurar servidores de bases de datos.

---

## 🛠️ Detalles de Implementación Técnica

El núcleo de este proyecto destaca por el uso práctico de conceptos fundamentales en ingeniería de software:
*   **Gestión de Flujos (I/O Streams):** Lectura y escritura optimizada utilizando clases nativas de .NET para mitigar bloqueos de archivos.
*   **Modelado Orientado a Objetos (POO):** Abstracción clara de entidades comerciales (Productos, Clientes, Facturas) mapeadas directamente desde texto plano.
*   **Parseo y Serialización Manual:** Procesamiento lineal de strings estructurados para transformar registros planos en objetos manipulables en memoria.

---

## 🚀 Cómo Ejecutar el Proyecto

### Requisitos previos
*   Tener instalado el **SDK de .NET 9**.

### Clonar y arrancar
1. **Clona el repositorio:**
   ```bash
   git clone https://github.com/AliaJimenez/BillCraft.git
