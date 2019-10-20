//
//  APDReviewVideo.h
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
#import <UIKit/UIKit.h>

#import <Appodeal/APDSdk.h>
#import <Appodeal/APDRewardProtocol.h>

#ifdef ADVANCED_INTEGRATION
#import <Appodeal/AppodealRequestDelegateProtocol.h>
#endif


@class APDRewardedVideo;

<<<<<<< HEAD
/**
 Declaration of rewarded video delegate
=======
/*!
 *  Declaration of rewarded video delegate
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
@protocol APDRewardedVideoDelegate <NSObject>

@optional
<<<<<<< HEAD
/**
 Method called when rewarded video did load
 @param rewardedVideo Ready to show rewarded video
 @param isPrecache Boolean flag that indicates that loaded rewarded video is preache or not
 */
- (void)rewardedVideoDidLoad:(nonnull APDRewardedVideo *)rewardedVideo isPrecache:(BOOL)isPrecache;
/**
 Method called if skippable rewarded mediation failed
 @param rewardedVideo Failed rewarded video
 @param error Error
 */
- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFailToLoadWithError:(nonnull NSError *)error;
/**
 Called when rewarded video ad expired
 @param rewardedVideo Rewarded video expired
 */
- (void)rewardedVideoDidExpired:(nonnull APDRewardedVideo *)rewardedVideo;
/**
 Method called after rewarded video shows
 @param rewardedVideo Shown rewarded video
 */
- (void)rewardedVideoDidAppear:(nonnull APDRewardedVideo *)rewardedVideo;
/**
 Method called after rewarded video is dismissed from screen
 @param rewardedVideo Shown rewarded video
 @param wasFullyWatched Fully watched
 */
- (void)rewardedVideoDidDisappear:(nonnull APDRewardedVideo *)rewardedVideo wasFullyWatched:(BOOL)wasFullyWatched;
/**
 Call when user taps on rewarded video
 @param rewardedVideo Shown rewarded video
 */
- (void)rewardedVideoDidRecieveTapAction:(nonnull APDRewardedVideo *)rewardedVideo;
/**
 Method called after rewarded video completes
 view and stay on screen.
 @warning After calling this method video controller can show postbanner
 @param rewardedVideo Completed rewarded video
 @param reward Object conformed APDReward protocol with values turned on in Appodeal Dashboard
 */
- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFinishWithReward:(nonnull id<APDReward>)reward;
/**
 Method called if an error occurs while presenting the rewarded video adapter
 @param rewardedVideo Failed rewarded video
 @param error Error occurred
 */
- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFailToPresentWithError:(nonnull NSError *)error;

@end
/**
 You should have strong reference on loading rewarded video instance
 Instance of rewarded video ad can try to load ad only once!
 Create new rewarded video before any call -loadAd!
 @note
 <pre> - (void) loadRewardedVideo {
 self.rewardedVideo = [APDRewaredVideo new];
 self.rewardedVideo.delegate = self;
 [self.rewardedVideo loadAd]
 }
 </pre>
 */
@interface APDRewardedVideo : NSObject
/**
 Set delegate to skippable video
 */
@property (weak, nonatomic, nullable) id<APDRewardedVideoDelegate> delegate;
/**
 Set custom SDK
 */
@property (weak, nonatomic, nullable) APDSdk *customSdk;
/**
 get predicated ecpm
=======
/*!
 *  Method called when rewarded video did load
 *
 *  @param rewardedVideo Ready to show rewarded video
 *  @param isPrecache Boolean flag that indicates that loaded rewarded video is preache or not
 *
 */
- (void)rewardedVideoDidLoad:(nonnull APDRewardedVideo *)rewardedVideo isPrecache:(BOOL)isPrecache;

/*!
 *  Method called if skippable rewarded mediation failed
 *
 *  @param rewardedVideo Failed rewarded video
 */
- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFailToLoadWithError:(nonnull NSError *)error;

/**
 Called when rewarded video ad expired

 @param rewardedVideo Rewarded video expired
 */
- (void)rewardedVideoDidExpired:(nonnull APDRewardedVideo *)rewardedVideo;

/*!
 *  Method called after rewarded video shows
 *
 *  @param rewardedVideo Shown rewarded video
 */
- (void)rewardedVideoDidAppear:(nonnull APDRewardedVideo *)rewardedVideo;

/*!
 *  Method called after rewarded video is dismissed from screen
 *
 *  @param rewardedVideo Shown rewarded video
 */
- (void)rewardedVideoDidDisappear:(nonnull APDRewardedVideo *)rewardedVideo wasFullyWatched:(BOOL)wasFullyWatched;

/*!
 *   Method called after rewarded video completes
 *   @warning After calling this method video controller can show postbanner
 *   view and stay on screen.
 *
 *  @param rewardedVideo Completed rewarded video
 *  @param reward        Object conformed APDReward protocol with values turned on in Appodeal Dashboard
 */
- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFinishWithReward:(nonnull id<APDReward>)reward;

/*!
 *  Method called if an error occurs while presenting the rewarded video adapter
 *
 *  @param rewardedVideo  Failed rewarded video
 *  @param error          Error occurred
 */

- (void)rewardedVideo:(nonnull APDRewardedVideo *)rewardedVideo didFailToPresentWithError:(nonnull NSError *)error;

@end

/*!
 *  You should have strong reference on loading rewarded video instance
 *  Instance of rewarded video ad can try to load ad only once!
 *  Create new rewarded video before any call -loadAd!
 *  @code - (void) loadRewardedVideo {
            self.rewardedVideo = [APDRewaredVideo new];
            self.rewardedVideo.delegate = self;
            [self.rewardedVideo loadAd]
        }
 */
@interface APDRewardedVideo : NSObject
/*!
 *  Set delegate to skippable video
 */
@property (weak, nonatomic, nullable) id<APDRewardedVideoDelegate> delegate;
///*!
// *  Get current placement name, that you create in the Appodeal Dashboard
// */
//@property (copy, nonatomic, readonly, nullable) NSString * placement;
/*!
 *  Set custom SDK
 */
@property (weak, nonatomic, nullable) APDSdk *customSdk;
/*!
 *  get predicated ecpm
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
@property (assign, nonatomic, readonly) double predictedEcpm;
/**
 Enable autocache
 */
@property (assign, nonatomic) BOOL autocache;
<<<<<<< HEAD
/**
 Get rewarded video availability
=======
/*!
 *  Get rewarded video availability
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
@property (assign, nonatomic, readonly, getter=isReady) BOOL ready;
/**
 Designated initializer
<<<<<<< HEAD
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 @param sdk Initialised sdk instance
 @param delegate delegate for rewarded video
 @param autocache enable or disable autocache
 @return ready for mediation instance of rewarded video
 */
+ (nonnull instancetype)rewardedVideoWithSdk:(nullable APDSdk *)sdk
                                    delegate:(nullable id<APDRewardedVideoDelegate>)delegate
                                   autocache:(BOOL)autocache;
<<<<<<< HEAD
/**
  Start loading rewarded video
 */
- (void)loadAd;
/**
 Show ready rewarded video from view controller
 @param viewController Current presented view controller
 @param placement Placement for present rewarded video
=======
/*!
 *  Start loading rewarded video
 */
- (void)loadAd;
/*!
 *  Show ready rewarded video from view controller
 *
 *  @param viewController Current presented view controller
 *  @param placement Placement for present rewarded video
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
- (void)presentFromViewController:(nonnull UIViewController *)viewController placement:(nonnull NSString *)placement;
/**
 Return reward object for given placement
<<<<<<< HEAD
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 @param placement Placement
 @return reward object
 */
- (nonnull id<APDReward>)rewardForPlacement:(nonnull NSString *)placement;
@end
