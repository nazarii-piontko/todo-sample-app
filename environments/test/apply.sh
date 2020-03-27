#!/bin/bash

helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# PostgreSQL
helm install todo-db bitnami/postgresql -f ${BASH_SOURCE%/*}/postgres.yaml --wait

# Selenium
helm install todo-bdd-driver stable/selenium  -f ${BASH_SOURCE%/*}/selenium.yaml --wait
nohup kubectl port-forward service/todo-bdd-driver-selenium-hub 4444:4444 >/dev/null 2>&1 &

# Application
helm install todo ${BASH_SOURCE%/*}/../../charts/app -f ${BASH_SOURCE%/*}/app.yaml --wait
