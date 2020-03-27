#!/bin/bash

helm delete todo

helm delete todo-bdd-driver

helm delete todo-db
kubectl delete data-todo-db-postgresql-0

pkill -f "kubectl port-forward service/todo-"
