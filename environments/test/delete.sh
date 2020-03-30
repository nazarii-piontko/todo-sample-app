#!/bin/bash

if [[ $1 == --app ]]; then
    # Application
    helm delete todo || true
    pkill -f "kubectl port-forward service/todo-app" || true

    # PostgreSQL
    helm delete todo-db || true
    kubectl delete pvc/data-todo-db-postgresql-0 || true
fi

if [[ $1 == --selenium ]]; then
    # Selenium
    helm delete todo-bdd-driver || true
    pkill -f "kubectl port-forward service/todo-bdd-driver-selenium-hub" || true
fi
