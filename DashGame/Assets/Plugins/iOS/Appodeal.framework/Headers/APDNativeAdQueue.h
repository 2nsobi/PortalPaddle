//
//  APDNativeAdQueue.h
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
#import <Appodeal/APDNativeAd.h>
#import <Appodeal/APDDefines.h>
#import <Appodeal/APDNativeAdSettings.h>
#import <Appodeal/APDSdk.h>


@class APDNativeAdQueue;
<<<<<<< HEAD
/**
 Declaration of native ad queue delegate
 */
@protocol APDNativeAdQueueDelegate <NSObject>

@optional
/**
 Method called when loader receives native ad.
=======


@protocol APDNativeAdQueueDelegate <NSObject>

@optional

/**
 Method called when loader receives native ad.
 
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 @param adQueue ad queue object
 @param count count of available native ad
 */
- (void)adQueueAdIsAvailable:(nonnull APDNativeAdQueue *)adQueue ofCount:(NSUInteger)count;
<<<<<<< HEAD
/**
 Method called when loader fails to receive native ad.
=======


/**
 Method called when loader fails to receive native ad.
 
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 @param adQueue ad queue object
 @param error Error occurred
 */
- (void)adQueue:(nonnull APDNativeAdQueue *)adQueue failedWithError:(nonnull NSError *)error;

@end
<<<<<<< HEAD
/**
  Instance of APDNativeAdQueue
 */
@interface APDNativeAdQueue : NSObject
/**
 Set loader delegate
 */
@property (nonatomic, weak, nullable) id<APDNativeAdQueueDelegate> delegate;
/**
 Set queue settings
 */
@property (nonatomic, strong, nonnull) APDNativeAdSettings * settings;
/**
 Get count of available native ads
 */
@property (nonatomic, readonly, assign) NSInteger currentAdCount;
/**
 Set current placement
 */
@property (nonatomic, strong, nullable) NSString * placement;
/**
 Set autocache
 */
@property (nonatomic, assign) BOOL autocache;
/**
 Get precache ad count
 */
@property (nonatomic, readonly, assign) NSInteger precacheAdCount;
/**
 Get avaiable ads count for placement > 0
 */
@property (nonatomic, readonly, assign) BOOL containsSuitableAdsForCurrentPlacement;
/**
 Set custom sdk
 */
@property (weak, nonatomic, nullable) APDSdk *customSdk;
/**
 Initializator
 @param sdk Current sdk
 @param settings Queue settings
 @param delegate Queue delegate
 @param autocache Autocache
 @return Instance of APDNativeAdQueue
 */
=======


@interface APDNativeAdQueue : NSObject

/*!
 *  Set loader delegate
 */
@property (nonatomic, weak, nullable) id<APDNativeAdQueueDelegate> delegate;

/*!
 *  Set queue settings
 */
@property (nonatomic, strong, nonnull) APDNativeAdSettings * settings;

/**
 * Get count of available native ads
 */
@property (nonatomic, readonly, assign) NSInteger currentAdCount;

@property (nonatomic, strong, nullable) NSString * placement;

@property (nonatomic, assign) BOOL autocache;

@property (nonatomic, readonly, assign) NSInteger precacheAdCount;

@property (nonatomic, readonly, assign) BOOL containsSuitableAdsForCurrentPlacement;

/*!
 *  Set custom sdk
 */
@property (weak, nonatomic, nullable) APDSdk *customSdk;

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
+ (nonnull instancetype)nativeAdQueueWithSdk:(nullable APDSdk *)sdk
                                    settings:(nonnull APDNativeAdSettings *)settings
                                    delegate:(nullable id<APDNativeAdQueueDelegate>)delegate
                                   autocache:(BOOL)autocache;
<<<<<<< HEAD
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
/**
 Set max count native ad
 @param adSize max count of native ad
 */
- (void)setMaxAdSize:(NSInteger)adSize __attribute__((deprecated("Configure ad queue size in dashboard")));
<<<<<<< HEAD
/**
 Call this method to load native ad.
 */
- (void)loadAd;
/**
 Call this method to get native ads
 @param count available native ads count
=======


/**
 * Call this method to load native ad.
 * @param type APDNativeAdTypeAuto or APDNativeAdTypeVideo or APDNativeAdTypeNoVideo
 */
- (void)loadAdOfType:(APDNativeAdType)type __attribute__((deprecated("Use -loadAd. Type of native ad defined in settings")));;

/**
 * Call this method to load native ad.
 */
- (void)loadAd;

/**
 Call this method to get native ads
 
 @param count count available native ads
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 @return array of native ad
 */
- (nonnull NSArray <__kindof APDNativeAd *> *)getNativeAdsOfCount:(NSInteger)count;


@end
