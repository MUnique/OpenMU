{{/* The name of the release, used as prefix of all resources. */}}
{{- define "openmu.fullname" -}}
{{- if contains .Chart.Name .Release.Name -}}
{{- .Release.Name | trunc 50 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name .Chart.Name | trunc 50 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}

{{- define "openmu.labels" -}}
app.kubernetes.io/name: {{ .Chart.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version }}
{{- end -}}

{{- define "openmu.selectorLabels" -}}
app.kubernetes.io/name: {{ .root.Chart.Name }}
app.kubernetes.io/instance: {{ .root.Release.Name }}
app.kubernetes.io/component: {{ .component }}
{{- end -}}

{{- define "openmu.image" -}}
{{- printf "%s/%s:%s" .root.Values.image.registry .image .root.Values.image.tag -}}
{{- end -}}

{{- define "openmu.databaseHost" -}}
{{- default (printf "%s-postgres" (include "openmu.fullname" .)) .Values.database.host -}}
{{- end -}}

{{- define "openmu.rabbitmqConnectionString" -}}
{{- default (printf "amqp://%s-rabbitmq:5672" (include "openmu.fullname" .)) .Values.rabbitmq.connectionString -}}
{{- end -}}

{{- define "openmu.redisHost" -}}
{{- default (printf "%s-redis:6379" (include "openmu.fullname" .)) .Values.redis.host -}}
{{- end -}}

{{/* The annotations which add a dapr sidecar to a pod. */}}
{{- define "openmu.daprAnnotations" -}}
dapr.io/enabled: "true"
dapr.io/app-id: {{ .appId | quote }}
dapr.io/app-port: "8080"
dapr.io/config: {{ printf "%s-dapr" (include "openmu.fullname" .root) | quote }}
{{- with .blockShutdownDuration }}
{{- /* The sidecar stays up until this duration elapsed, or until the app stops answering its health endpoint. */}}
dapr.io/block-shutdown-duration: {{ . | quote }}
dapr.io/enable-app-health-check: "true"
dapr.io/app-health-check-path: "/health/live"
dapr.io/app-health-probe-interval: "2"
dapr.io/app-health-threshold: "2"
{{- end }}
{{- end -}}

{{/* The environment variables which every OpenMU container gets. */}}
{{- define "openmu.commonEnv" -}}
- name: ASPNETCORE_URLS
  value: "http://+:8080"
{{- with .Values.otlpEndpoint }}
- name: OTEL_EXPORTER_OTLP_ENDPOINT
  value: {{ . | quote }}
{{- end }}
{{- end -}}

{{/* The probes of every OpenMU container, see the health endpoints in the docs. */}}
{{- define "openmu.probes" -}}
startupProbe:
  httpGet:
    path: /health/live
    port: http
  periodSeconds: 5
  failureThreshold: 60
livenessProbe:
  httpGet:
    path: /health/live
    port: http
  periodSeconds: 10
  failureThreshold: 6
readinessProbe:
  httpGet:
    path: /health/ready
    port: http
  periodSeconds: 10
  timeoutSeconds: 5
  failureThreshold: 3
{{- end -}}
