#!/bin/sh

echo "Aguardando MinIO ficar disponível..."

until mc alias list; do
  echo "MinIO ainda não está pronto. Aguardando..."
  sleep 3
done

echo "MinIO disponível! Configurando bucket..."

mc alias set local http://minio:9000 minioadmin minioadmin
mc mb local/moto-service || echo "Bucket já existe"
mc policy set public local/moto-service

echo "Bucket moto-service configurado com sucesso!"
