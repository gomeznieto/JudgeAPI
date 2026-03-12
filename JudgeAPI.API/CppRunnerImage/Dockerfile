# Imagen base con Debian slim (liviana)
FROM debian:stable-slim

# Instalar compilador y utilidades básicas
RUN apt-get update && apt-get install -y --no-install-recommends \
    g++ \
    make \
    time \
    procps \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Crear usuario no-root (más seguro)
RUN useradd -ms /bin/bash runner
USER runner

# Carpeta de trabajo
WORKDIR /app/jobs
USER 1000:1000
# EntryPoint vacío (el contenedor va a recibir comandos como `g++ main.cpp`)
CMD ["/bin/bash"]
