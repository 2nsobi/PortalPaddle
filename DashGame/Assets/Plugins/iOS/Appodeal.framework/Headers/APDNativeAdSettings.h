//
//  APDNativeAdSettings.h
//  Appodeal
//
<<<<<<< HEAD
//  Created by Stas Kochkin on 04/07/2019.
//  Copyright © 2019 Appodeal, Inc. All rights reserved.
=======
//  Created by Stas Kochkin on 04/07/2018.
//  Copyright © 2018 Appodeal, Inc. All rights reserved.
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
//

#import <Foundation/Foundation.h>
#import <Appodeal/APDNativeAdViewProtocol.h>
#import <Appodeal/APDDefines.h>


<<<<<<< HEAD
/**
 Native resource autocache mask
 */
typedef NS_OPTIONS(NSUInteger, APDNativeResourceAutocacheMask) {
    /**
     Autocache icon
     */
    APDNativeResourceAutocacheIcon = 1 << 0,
    /**
     Autocache Media
     */
    APDNativeResourceAutocacheMedia = 1 << 1
};

/**
 Instance of APDNativeAdSettings
 */
@interface APDNativeAdSettings : NSObject
/**
 Set native ad view class
 */
@property (nonatomic, assign, nonnull) Class <APDNativeAdView> adViewClass;
/**
 Set native ad Type
 */
@property (nonatomic, assign) APDNativeAdType type;
/**
 Set autocache mask
 */
@property (nonatomic, assign) APDNativeResourceAutocacheMask autocacheMask;
/**
 Set native placehplder URL
 This url use as pleceholder for mainImage view
 */
@property (nonatomic, strong, nullable) NSURL * nativeMediaViewPlaceholder;
/**
 Default instance
 @return Instance of APDNativeAdSettings
 */
=======
typedef NS_OPTIONS(NSUInteger, APDNativeResourceAutocacheMask) {
    APDNativeResourceAutocacheIcon = 1 << 0,
    APDNativeResourceAutocacheMedia = 1 << 1
};

@interface APDNativeAdSettings : NSObject

@property (nonatomic, assign, nonnull) Class <APDNativeAdView> adViewClass;
@property (nonatomic, assign) APDNativeAdType type;
@property (nonatomic, assign) APDNativeResourceAutocacheMask autocacheMask;

@property (nonatomic, strong, nullable) NSURL * nativeMediaViewPlaceholder;

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
+ (instancetype _Nonnull)defaultSettings;

@end
