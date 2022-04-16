GOOGLE_JSON_FILE=$APPCENTER_SOURCE_DIRECTORY/BusSchedule.Android/Assets/google-services.json

if [ -e "$GOOGLE_JSON_FILE" ]
then
    echo "Updating Google Json"
    echo "$GOOGLE_SERVICES_JSON" > $GOOGLE_JSON_FILE
    sed -i -e 's/\\"/'\"'/g' $GOOGLE_JSON_FILE

    #echo "File content:"
    #cat $GOOGLE_JSON_FILE
fi

touch $APPCENTER_SOURCE_DIRECTORY/BusSchedule.Android/Resources/values/keys.xml
KEYS_FILE=$APPCENTER_SOURCE_DIRECTORY/BusSchedule.Android/Resources/values/keys.xml
if [ -e "$KEYS_FILE" ]
then
    echo "Updating keys.xml"
    echo "$KEYS_FILE_CONTENT" > $KEYS_FILE
    sed -i -e 's/\\"/'\"'/g' $KEYS_FILE

    #echo "File content:"
    #cat $KEYS_FILE
fi