<h1 align="center">🧾 BillCraft</h1>

<p align="center">
  <b>Fast inventory and invoicing system for the console.</b><br/>
  <i>Sistema de inventario y facturación rápido para consola.</i>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/.NET-9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9"/>
  <img src="https://img.shields.io/badge/Storage-Text_files-orange?style=for-the-badge" alt="Text files"/>
</p>

---

## ✨ Features · Funcionalidades

| EN | ES |
|---|---|
| Product management: add products and browse the catalog | Gestión de productos: agregar productos y consultar el catálogo |
| New sale: pick products and quantities, totals calculated automatically | Nueva venta: elige productos y cantidades, con totales automáticos |
| Sequential invoice numbers (`F-0001`, `F-0002`…) | Numeración secuencial de facturas |
| Invoice history with full detail | Historial de facturas con su detalle |
| Input validation on every prompt | Validación de entradas en cada paso |
| Zero setup: data lives in `productos.txt` and `facturas.txt` | Sin configuración: los datos viven en archivos de texto |

## 🛠️ Technical highlights · Detalles técnicos

- Object-oriented model: `Producto`, `Factura` and `DetalleFactura`.
- Manual parsing / serialization of pipe-delimited text records.
- Fixed `es-DO` culture so dates and amounts are read and written the same way on any machine.

## 🚀 Run · Ejecutar

**Requirements · Requisitos:** .NET 9 SDK.

```bash
git clone https://github.com/AliaJimenez/BillCraft.git
cd BillCraft/SistemadeFacturación
dotnet run
```

The repository includes sample products and invoices so you can try it right away.
*El repositorio incluye productos y facturas de ejemplo para probarlo al instante.*

## 👩‍💻 Author · Autora

**Aliandy Jimenez** — [@AliaJimenez](https://github.com/AliaJimenez)
