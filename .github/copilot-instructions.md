# Copilot Instructions para Windows Forms en .NET 10

## Objetivo
Este proyecto es una aplicación de lista de tareas de escritorio con Windows Forms en C# y .NET 10. Mantén el código limpio, mantenible y alineado con las buenas prácticas de desarrollo de escritorio.

## Estilo y arquitectura
- Usa C# moderno con .NET 10.
- Mantén una arquitectura en capas simple y clara: formularios/Interfaz de usuario, servicios de aplicación y modelos de dominio.
- Los formularios deben manejar solo interacción con el usuario; la lógica de negocio, validaciones y acceso a datos debe residir en servicios o repositorios.
- Si se añade persistencia, usa repositorios o servicios dedicados y evita mezclar acceso a datos directamente en los formularios.
- Mantén los formularios enfocados en la interfaz de usuario y la interacción con el usuario, delegando la lógica de negocio a servicios o casos de uso.
- Usa servicios para encapsular la lógica de negocio, validaciones, acceso a datos y operaciones reutilizables.
- Evita poner lógica compleja dentro de los eventos de los controles; extrae esa lógica a métodos, clases de servicio o helpers.
- No mezcles lógica de negocio directamente en los eventos de la interfaz de usuario; mantén los formularios delgados y delega la lógica a servicios o casos de uso.
- Sigue el patrón de inyección de dependencias cuando sea posible, especialmente para servicios y repositorios.
- Usa nombres descriptivos para formularios, clases, métodos y controles.

## Convenciones recomendadas
- Nombres en PascalCase para clases, métodos, propiedades y formularios.
- Nombres en camelCase para variables locales y parámetros.
- Usa `async`/`await` para operaciones de E/S, acceso a datos y llamadas HTTP.
- Mantén los formularios responsivos y evita bloquear la interfaz de usuario con operaciones largas.
- Usa validaciones de entrada en los formularios y en los servicios para evitar inconsistencias.
- Gestiona correctamente eventos como `Click`, `Load`, `FormClosing` y `Validating`.

## Diseño de interfaz de escritorio
- Usa Windows Forms para la interfaz de usuario.
- Prefiere paneles, UserControl y clases auxiliares para organizar la UI de forma reutilizable.
- Mantén la estructura de los formularios clara y coherente.
- Mantén los formularios delgados: la interfaz debe orquestar acciones, no contener la lógica de dominio.
- Evita lógica de negocio directamente en los formularios; usa ViewModels/DTOs o clases de presentación cuando sea necesario.

## Datos y acceso a datos
- Si usas Entity Framework Core, coloca la configuración en `DbContext` y usa migraciones para cambios de esquema.
- Usa repositorios o servicios para abstraer el acceso a datos.
- No expongas directamente entidades de base de datos a la interfaz de usuario; usa modelos de vista, DTOs o clases de presentación.

## Estructura recomendada del proyecto
- Organiza el código en carpetas como `Forms/`, `Services/`, `Models/`, `Repositories/` y `Utilities/` cuando sea necesario.
- Mantén una separación clara entre interfaz, lógica de negocio y acceso a datos.
- Evita crear clases dispersas; agrupa la funcionalidad por responsabilidad.

## Principios de simplicidad
- Evita sobreingeniería: usa soluciones simples y directas para una app de lista de tareas de escritorio.
- No añadas frameworks, patrones o capas complejas si no aportan valor real al proyecto.
- Prioriza claridad, mantenibilidad y velocidad de desarrollo sobre abstracciones innecesarias.
- Mantén la implementación alineada con el tamaño y propósito de la aplicación.

## Experiencia de usuario
- Diseña la interfaz para que sea clara, rápida y fácil de usar en escritorio.
- Prioriza flujos simples para crear, editar, completar y eliminar tareas.
- Usa mensajes y etiquetas comprensibles para el usuario final.

## Estilo visual: Microsoft To Do (Fluent Design)
El diseño visual debe inspirarse en Microsoft To Do. Características clave a respetar:

### Colores y tipografía
- Color de acento principal: `#2564CF` (azul Microsoft To Do). Secundario oscuro: `#1A4A9C`.
- Fondo general: `#F3F2F1`. Fondo de ítems: `Color.White`.
- Texto principal: `#1F1F1F`. Texto secundario/metadata: `#767676`.
- Fuente base: `Segoe UI`, 10pt para ítems, 13pt bold para títulos/encabezados.
- Tareas completadas: texto tachado y atenuado `#A0A0A0`.

### Layout del formulario principal
- Header superior: altura 56px, fondo azul acento, muestra el nombre de la vista activa en blanco bold 14pt.
- Panel de filtros: fondo `#EDEBE9`, altura 40px, combos planos.
- Lista de tareas: `FlowLayoutPanel` vertical con scroll, fondo `#F3F2F1`. No usar `DataGridView`.
- Add-bar inferior: altura 48px, fondo blanco, borde superior sutil. Contiene `TextBox` "Añadir una tarea" y botón `+` plano.

### Ítems de tarea (`TareaItemControl` UserControl)
- Altura fija: 52px. Fondo blanco con borde inferior `#E1DFDD`.
- Checkbox circular dibujado con GDI+, 20px, borde `#767676`, relleno azul acento cuando está completada.
- Título: Segoe UI 10pt. Tachado + color atenuado si está completada.
- Metadata: `Segoe UI` 8pt, `#767676` con categoría · prioridad · fecha (roja si vencida).
- Estrella `★/☆`: amarilla `#F8C537` si prioridad Alta, gris si no.
- Hover: fondo `#F0EEF8`. Doble clic abre edición.

### Botones y controles
- Botones planos (`FlatStyle.Flat`) sin borde, `Segoe UI` 9pt.
- Botón principal: fondo acento azul, texto blanco.
- Botones secundarios: fondo transparente, texto `#2564CF`.

### Formularios modales
- Fondo blanco. Header azul con título bold, campos con etiqueta pequeña encima y TextBox con solo borde inferior.

### Clases de utilidad recomendadas
- `Utilities/AppTheme.cs` con colores, fuentes y helpers (`ApplyFlatButton`, `DrawCircularCheckbox`).
- Reutilizar `AppTheme` en formularios y controles.

## Calidad del código
- Habilita y respeta `nullable reference types`.
- Añade validaciones, manejo de errores y mensajes claros para el usuario.
- Si añades pruebas, manténlas centradas en servicios y lógica de negocio, no solo en la interfaz gráfica.
- Usa logging con `ILogger` cuando sea adecuado.
- Maneja excepciones de forma controlada y evita mostrar detalles internos al usuario.
- Evita código duplicado; extrae lógica reutilizable a métodos, servicios o utilidades.
- Mantén los métodos pequeños, fáciles de probar y fáciles de mantener.

## Respuestas esperadas de Copilot
Cuando generes código para este proyecto:
- Propón soluciones compatibles con aplicaciones de escritorio Windows Forms en .NET 10.
- Usa servicios, repositorios y modelos de presentación cuando sea apropiado.
- Mantén el enfoque en claridad, usabilidad, seguridad y mantenibilidad.
- Si hay ambigüedad, explica brevemente la opción recomendada.

## Instrucciones para Copilot
- Esta aplicación debe centrarse en gestionar tareas: crear, editar, marcar como completadas, eliminar y filtrar elementos de una lista de tareas.
- Cuando generes código para este proyecto, asegúrate de que la lógica de tareas, estados y categorías esté bien separada en modelos, servicios y formularios.
- Prioriza soluciones que funcionen bien en entornos de escritorio Windows, sean fáciles de usar y sean fáciles de mantener a medio plazo.
