#!/bin/bash

helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

if [[ $1 == --app ]]; then
    # PostgreSQL
    echo "Installing PostgreSQL..."
    helm install todo-db bitnami/postgresql -f ${BASH_SOURCE%/*}/postgres.yaml --wait

    # Application
    echo "Installing application..."
    helm install todo ${BASH_SOURCE%/*}/../../charts/app -f ${BASH_SOURCE%/*}/app.yaml --wait
    echo "Enabling port forward for Application to port 5000: http://localhost:5000/"
    nohup kubectl port-forward service/todo-app 5000:80 >/dev/null 2>&1 &
fi

if [[ $1 == --selenium ]]; then
    # Selenium
    echo "Installing Selenium..."
    helm install todo-bdd-driver stable/selenium  -f ${BASH_SOURCE%/*}/selenium.yaml --wait
    echo "Enabling port forward for Selenium to port 4444: http://localhost:4444/"
    nohup kubectl port-forward service/todo-bdd-driver-selenium-hub 4444:4444 >/dev/null 2>&1 &
fi