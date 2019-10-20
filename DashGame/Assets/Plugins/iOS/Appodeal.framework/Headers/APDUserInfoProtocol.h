//
//  APDUserInfoProtocol.h
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
#import <Appodeal/APDDefines.h>

@protocol APDUserInfo <NSObject>
<<<<<<< HEAD
/**
 Age
 */
@property (assign, nonatomic, readonly) NSUInteger age;
/**
 Gender
 */
@property (assign, nonatomic, readonly) APDUserGender gender;
/**
 UserId
 */
@property (copy, nonatomic, readonly) NSString *userId;
/**
 Ext
 */
@property (copy, nonatomic, readonly) NSDictionary *ext;

=======

@property (copy, nonatomic, readonly) NSString *email;
@property (copy, nonatomic, readonly) NSArray *interests;
@property (copy, nonatomic, readonly) NSString * userId;
@property (copy, nonatomic, readonly) NSDictionary *ext;

@property (strong, nonatomic, readonly) NSDate *birthday;
@property (strong, nonatomic, readonly) NSString *birthdayString;
@property (assign, nonatomic, readonly) NSUInteger age;
@property (assign, nonatomic, readonly) APDUserGender gender;
@property (assign, nonatomic, readonly) APDUserOccupation occupation;
@property (assign, nonatomic, readonly) APDUserRelationship relationship;
@property (assign, nonatomic, readonly) APDUserSmokingAttitude smokingAttitude;
@property (assign, nonatomic, readonly) APDUserAlcoholAttitude alcoholAttitude;
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

@end
