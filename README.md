
# JudgeAPI: Plataforma de Evaluación de Código

Este documento describe la arquitectura y el flujo de trabajo de JudgeAPI, una plataforma diseñada para la creación y evaluación automática de ejercicios de programación.

## Herramientas y Tecnologías

La plataforma está construida sobre un ecosistema de .NET y contenedores, aprovechando las siguientes tecnologías:

-   **Backend:** ASP.NET Core 9
-   **Base de Datos:** MS SQL Server
-   **ORM:** Entity Framework Core
-   **Cache y Cola de Mensajes:** Redis
-   **Arquitectura:** Microservicios orquestados con Docker Compose
-   **Aislamiento de Ejecución:** Contenedores Docker para compilación y ejecución segura de código C++.

## Arquitectura y Flujo de Ejecución

El sistema está diseñado para ser robusto, escalable y seguro, desacoplando la recepción de envíos de su ejecución y evaluación. Esto se logra a través de una comunicación asíncrona utilizando Redis como cola de trabajos.

A continuación, se detalla el flujo completo, desde que un usuario envía su código hasta que recibe un veredicto.

![Diagrama de Flujo de JudgeAPI](https://i.imgur.com/your-diagram-image.png) <!-- Reemplazar con una URL de diagrama si se tiene -->

### 1. Recepción del Envío (Submission)

1.  **Endpoint API:** El usuario envía su código fuente (actualmente C++) a un endpoint protegido de la **API de Judge**.
2.  **Creación de la Entidad:** El `SubmissionController` recibe la petición, la asocia al usuario y al problema correspondiente, y crea una nueva entidad `Submission` en la base de datos con un estado inicial.
3.  **Encolado en Redis:** El servicio `DistributedAnalyzer` es invocado. Este actualiza el estado del `Submission` a **"En Cola"** (`Queued`) y publica un nuevo trabajo en una lista de Redis (`submissions`). El mensaje contiene el ID del envío que necesita ser procesado.

### 2. Procesamiento por el Runner

1.  **Escucha Activa:** El servicio **Runner** (`RunnerApp`), un worker de .NET, está constantemente escuchando la lista `submissions` en Redis.
2.  **Recepción del Trabajo:** Cuando un nuevo trabajo aparece, el Runner lo toma de la cola y deserializa el mensaje para obtener el `SubmissionId`.
3.  **Preparación del Entorno Aislado:**
    *   Se crea un directorio temporal único para el trabajo dentro de un volumen de Docker (`jobsdata`).
    *   El código fuente del usuario se escribe en un archivo `main.cpp` dentro de este directorio.

### 3. Fase de Compilación

1.  **Contenedor de Compilación:** El Runner invoca un comando `docker run` para crear un nuevo contenedor a partir de una imagen de C++ (`CppRunnerImage`). Este contenedor tiene acceso de solo lectura al código fuente.
2.  **Compilación Segura:** Dentro del contenedor, se ejecuta `g++` para compilar el código. El contenedor tiene recursos limitados (CPU, memoria) y no tiene acceso a la red para evitar cualquier riesgo de seguridad.
3.  **Manejo de Error de Compilación:**
    *   Si el proceso de compilación falla (código de salida no es cero), la ejecución se detiene.
    *   El Runner captura el error estándar (`stderr`), lo normaliza para eliminar rutas de sistema y lo guarda en la base de datos.
    *   El veredicto del `Submission` se actualiza a **"Error de Compilación"** (`CompilationError`), y el flujo termina.

### 4. Fase de Ejecución y Evaluación

Si la compilación es exitosa, el Runner procede a ejecutar el programa compilado contra cada caso de prueba (`TestCase`) asociado al problema.

1.  **Iteración de Casos de Prueba:** Para cada caso de prueba:
    *   Se escribe el dato de entrada (`Input`) del caso de prueba en un archivo `input.txt` en el directorio del trabajo.
    *   Se invoca un nuevo contenedor Docker para ejecutar el programa compilado. Este contenedor tiene restricciones aún más estrictas:
        *   **Tiempo Límite (Timeout):** El comando de ejecución está envuelto en un `timeout` (ej. `timeout 1s ./a.out`).
        *   **Límites de Recursos:** Se aplican límites estrictos de CPU y memoria (`--cpus`, `--memory`).
        *   **Sin Red:** `--network none`.
        *   **Sistema de Archivos de Solo Lectura:** `--read-only`, con excepciones para `stdin`, `stdout` y `tmpfs`.
    *   La salida estándar (`stdout`) del programa se redirige a un archivo `output.txt`.

2.  **Análisis del Resultado:** Una vez que el contenedor termina, el Runner analiza el resultado:
    *   **Time Limit Exceeded (TLE):** Si el proceso fue terminado por `timeout` (código de salida 124), el veredicto es TLE, y la evaluación de los demás casos de prueba se detiene.
    *   **Memory Limit Exceeded (MLE):** Si el proceso fue terminado por el kernel debido a un uso excesivo de memoria (código de salida 137), el veredicto es MLE.
    *   **Runtime Error (RE):** Si el programa termina con un código de salida distinto de cero (y no es TLE o MLE), se considera un error en tiempo de ejecución. El error estándar (`stderr`) se puede guardar para depuración.
    *   **Wrong Answer (WA):** Si el programa se ejecuta correctamente, el contenido de `output.txt` se normaliza (eliminando espacios en blanco y saltos de línea inconsistentes) y se compara con la salida esperada (`ExpectedOutput`) del caso de prueba. Si no coinciden, el veredicto es WA.
    *   **Correct (AC):** Si la salida coincide, el caso de prueba se marca como correcto.

### 5. Veredicto Final

1.  **Agregación de Resultados:** Después de ejecutar todos los casos de prueba (o detenerse prematuramente por un error), el Runner determina el veredicto final del `Submission`:
    *   **Correct:** Si todos los casos de prueba pasaron.
    *   **Time Limit Exceeded / Runtime Error / etc.:** El veredicto del primer caso de prueba que falló.
    *   **Wrong Answer:** Si ningún caso de prueba causó un error, pero al menos uno no produjo la salida correcta.
2.  **Actualización en Base de Datos:** Todos los resultados detallados por caso de prueba (`SubmissionResult`) y el veredicto final del `Submission` se guardan en la base de datos.
3.  **Limpieza:** El directorio temporal del trabajo, junto con el código fuente y los ejecutables, se elimina por completo.

El usuario ahora puede consultar el resultado final y ver el detalle de cada caso de prueba a través de la API.
