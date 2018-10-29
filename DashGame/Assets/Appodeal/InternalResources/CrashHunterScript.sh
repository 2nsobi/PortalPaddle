##!/bin/bash

if [ "$CONFIGURATION" == "Release" ]
then
exit 0
fi

UPLOADER_PATH=$(find $PROJECT_DIR/.. -type f -iname dSYMUploader | sort | uniq | head -n 1)
echo ${UPLOADER_PATH}

if [ -e ${UPLOADER_PATH} ]
then
${UPLOADER_PATH} ${BUILT_PRODUCTS_DIR} ${INFOPLIST_PATH} ${PRODUCT_BUNDLE_IDENTIFIER}
fi
