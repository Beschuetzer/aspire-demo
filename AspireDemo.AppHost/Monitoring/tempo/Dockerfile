FROM grafana/tempo:2.5.0

USER root
COPY tempo-config.yaml /etc/tempo/local-config.yaml
RUN mkdir -p /var/tempo && chown -R tempo:tempo /var/tempo
USER tempo
