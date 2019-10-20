//
//  APDImage.h
//  Appodeal
//
<<<<<<< HEAD
//  AppodealSDK version 2.5.11
//
//  Copyright © 2019 Appodeal, Inc. All rights reserved.
=======
//  AppodealSDK version 2.4.8.1-Beta
//
//  Copyright © 2018 Appodeal, Inc. All rights reserved.
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

<<<<<<< HEAD
/**
 Instance of this class contains URL to image source and size of image
 */
@interface APDImage : NSObject
/**
 Size of image. Can be zero
 */
@property (nonatomic, assign) CGSize size  __attribute__((deprecated("This getter is deprecated and will be removed in next release")));
/**
 Url to image source. Can be local
=======
/*!
 *  Instance of this class contains URL to image source and size of image
 */
@interface APDImage : NSObject

/*!
 *  Size of image, can be APDImageUndefined
 */
@property (nonatomic, assign) CGSize size;

/*!
 *  Url to image source. Can be local
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
@property (nonatomic, strong, readonly, nonnull) NSURL * url;

@end
