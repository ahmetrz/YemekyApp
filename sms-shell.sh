#!/bin/sh

exec 3>&1 4>&2
trap 'exec 2>&4 1>&3' 0 1 2 3
exec 1>/usr/lib/zabbix/alertscripts/log/log.out 2>&1

USERNAME=xxxx
PASSWORD=xxxx
PHONE=$1
MESSAGE=$2

echo "date +%H:%M:%: Sending SMS Text to $1"

curl -X POST "http://otpsms.postaguvercini.com/api_http/sendbulksms.asp?user=xxxx&password=xxxx&gsm=$PHONE" --data-urlencode text="$MESSAGE"

echo "date +%H:%M:%: SMS Text sent to $1"
echo "MESSAGE: $2"
