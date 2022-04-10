FROM mcr.microsoft.com/dotnet/runtime:6.0
RUN mkdir /app
WORKDIR /app
COPY publish ./
ENTRYPOINT [ "dotnet", "TraceviewZobalesaz.dll" ]