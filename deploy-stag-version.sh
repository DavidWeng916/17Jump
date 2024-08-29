#!/bin/bash

BUCKET_URL="gs://17live-game-sta"
BUCKET_NAME="POC/Jump/v3-demo"

functions_array=(
# "removeFolderContent 0" # uncomment this if you want to remove the all the Build folder content
# "copyToGoogle ./Builds/WebGL/main.js $BUCKET_URL/$BUCKET_NAME/"
"copyFolderToGoogle ./Builds/WebGL/TemplateData $BUCKET_URL/$BUCKET_NAME/"
"copyFolderToGoogleGzip ./Builds/WebGL/Build $BUCKET_URL/$BUCKET_NAME/"
"copyToGoogle ./Builds/WebGL/Build/WebGL.loader.js $BUCKET_URL/$BUCKET_NAME/Build/"
"copyIndex 0")

copyFolderToGoogleGzip() {
    # echo $1 $2
    gsutil -h "Cache-Control:no-cache, public" -h "Content-encoding:gzip" cp -r $1 $2
}

copyFolderToGoogle() {
    # echo $1 $2
    gsutil -h "Cache-Control:no-cache, public" cp -r $1 $2
}

copyIndex() {
    # echo $1 $2
    gsutil -h "Cache-Control:no-cache, public" cp ./Builds/WebGL/index.html $BUCKET_URL/$BUCKET_NAME/
}

copyToGoogle() {
    # echo $1 $2
    gsutil -h "Cache-Control:no-cache, public" cp $1 $2
}

# removeFolderContent() {
#     # echo $1 $2
#     # gsutil rm -r $BUCKET_URL/$BUCKET_NAME/Build/
# }

BAR='################################################'   # this is full bar, e.g. 20 chars
let p=0
echo [ Deploy Started ]
for i in "${functions_array[@]}"; do
    echo -ne "\rProgress: #${BAR:0:$p}\r"
    ${i} > progress.log
((p=p+5))
done
printf "\n [ OUTPUT ] \n"
cat progress.log
printf "\n"
printf "\n[ Deploy Complete ]"