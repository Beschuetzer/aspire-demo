FROM grafana/loki:2.8.2

USER root
COPY loki-config.yaml /etc/loki/local-config.yaml
RUN mkdir -p /loki /wal
# Run as root to avoid permission issues with mounted volumes
