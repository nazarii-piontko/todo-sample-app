#!/bin/sh
# Alpine linux only

set -e

apk update
apk add make curl gettext

curl -o /tmp/helm.tar.gz https://get.helm.sh/helm-v3.1.2-linux-amd64.tar.gz
tar -zxvf /tmp/helm.tar.gz -C /tmp/
mv /tmp/linux-amd64/helm /usr/local/bin/helm

curl -o /tmp/kubectl -LO https://storage.googleapis.com/kubernetes-release/release/`curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt`/bin/linux/amd64/kubectl
mv /tmp/kubectl /usr/local/bin/kubectl
chmod +x /usr/local/bin/kubectl
