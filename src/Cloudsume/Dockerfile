FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y curl libcap2-bin

WORKDIR /opt/cloudsume-server
COPY . ./

RUN useradd -d /opt/cloudsume-server -M -r -s /usr/sbin/nologin aspnet
RUN setcap 'cap_net_bind_service=+ep' /usr/share/dotnet/dotnet

RUN mkdir -pm 0750 /var/opt/cloudsume-server && chown aspnet:aspnet /var/opt/cloudsume-server
VOLUME /var/opt/cloudsume-server

USER aspnet

# override settings
ENV Logging__Console__FormatterName=Simple

ENTRYPOINT ["dotnet", "Cloudsume.dll"]
