//
//  APDRewardProtocol.h
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


<<<<<<< HEAD
/**
 Declaration of Appodeal reward protocol object
 */
@protocol APDReward <NSObject>
/**
 App currency name. For example @"Coins", @"Stars"
 */
@property (copy, nonatomic, readonly, nullable) NSString *currencyName;
/**
 App currency amount
 */
@property (assign, nonatomic, readonly) float amount;
=======
/*!
 *  Declaration of Appodeal reward protocol object
 */
@protocol APDReward <NSObject>

/*!
 *  App currency name. For example @"Coins", @"Stars"
 */
@property (copy,   nonatomic, readonly, nullable) NSString *currencyName;

/*!
 *  App currency amount
 */
@property (assign, nonatomic, readonly) NSUInteger amount;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

@end
