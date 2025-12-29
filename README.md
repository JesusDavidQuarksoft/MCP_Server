# Guía de Instalación y Ejecución

## Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (opcional, pero recomendado para desarrollo)
- Acceso a la terminal o línea de comandos

## Descarga del Proyecto

Puedes clonar el repositorio desde tu sistema de control de versiones (por ejemplo, GitHub):

```
git clone <URL_DEL_REPOSITORIO>
cd MCP_Extencion
```

## Restaurar Dependencias

Restaura los paquetes NuGet necesarios:

```
dotnet restore
```

## Compilar el Proyecto

Compila el proyecto usando el siguiente comando:

```
dotnet build
```

## Ejecutar el Servicio

Para ejecutar el servicio, utiliza:

```
dotnet run --project MCP_Extencion.csproj
```

O si prefieres usar Visual Studio:

1. Abre el archivo `MCP_Extencion.sln` en Visual Studio.
2. Selecciona la configuración `Debug` y la plataforma `Any CPU` o `x64`.
3. Presiona `F5` para iniciar la depuración o `Ctrl+F5` para ejecutar sin depuración.

## Configuración

Si necesitas configurar el entorno, revisa el archivo `Properties/launchSettings.json` para parámetros de inicio y perfiles de ejecución.

## Notas Adicionales

- Los archivos de salida se generan en la carpeta `bin/Debug/net8.0`.
- Si el servicio requiere configuración adicional, revisa los archivos dentro de la carpeta `Properties`.
