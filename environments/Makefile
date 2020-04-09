ENVIRONMENT ?= local

KUBE_NAMESPACE ?= default
KUBE_DELETE_PVC ?= true

IMAGE_BUILD_TAG ?= latest

BASE_DIR := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))

define install
	$(eval HELM_VARS_TMP_FILE := $(shell mktemp))
	envsubst < "$(BASE_DIR)/$(ENVIRONMENT)/$(3)" > "$(HELM_VARS_TMP_FILE)"

	helm upgrade $(1) $(2) --install --wait -f "$(HELM_VARS_TMP_FILE)" $(if $(KUBE_NAMESPACE),-n $(KUBE_NAMESPACE))

	-rm -f "$(HELM_VARS_TMP_FILE)"
endef

define forward
	kubectl $(if $(KUBE_NAMESPACE),--namespace $(KUBE_NAMESPACE)) port-forward $(1) $(2) >/dev/null 2>&1 &
endef

define delete
	helm delete $(1) || true
	ps aux | grep '\w port-forward $(2) \d' | awk '{print $$1}' | xargs kill || true
endef

build-image:
	cd "$(BASE_DIR)/../" && docker build -t registry.gitlab.com/nazarii-piontko/todo-sample-app:$(IMAGE_BUILD_TAG) .

repo-update:
	helm repo add stable https://kubernetes-charts.storage.googleapis.com/
	helm repo add bitnami https://charts.bitnami.com/bitnami
	helm repo update

install-db: repo-update
	$(call install,todo-db,bitnami/postgresql,postgres.yaml)

forward-db:
	$(call forward,service/todo-db-postgresql,5432:5432)

install-db-ui: repo-update
	$(call install,todo-db-ui,stable/pgadmin,pgadmin.yaml)

forward-db-ui:
	$(call forward,service/todo-db-ui-pgadmin,5433:80)

install-selenium: repo-update
	$(call install,todo-bdd-driver,stable/selenium,selenium.yaml)

forward-selenium:
	$(call forward,service/todo-bdd-driver-selenium-hub,4444:4444)

install-app: repo-update
	$(call install,todo,"$(BASE_DIR)/../charts/app",app.yaml)

forward-app:
	$(call forward,service/todo-app,5000:80)

delete-db:
	$(call delete,todo-db,service/todo-db-postgresql)
ifeq ($(KUBE_DELETE_PVC), true)
	kubectl delete pvc/data-todo-db-postgresql-0 || true
endif

delete-db-ui:
	$(call delete,todo-db-ui,service/todo-db-ui-pgadmin)

delete-selenium:
	$(call delete,todo-bdd-driver,service/todo-bdd-driver-selenium-hub)

delete-app: 
	$(call delete,todo,service/todo-app)

delete: delete-db delete-db-ui delete-selenium delete-app

.PHONY: build-image repo-update install-db forward-db install-db-ui forward-db-ui install-selenium forward-selenium install-app forward-app delete-db delete-db-ui delete-selenium delete-app delete
