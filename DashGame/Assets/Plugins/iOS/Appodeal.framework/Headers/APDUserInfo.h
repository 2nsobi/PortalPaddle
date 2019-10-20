//
//  APDUserInfo.h
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
#import <Appodeal/APDUserInfoProtocol.h>

/**
<<<<<<< HEAD
 Instance of class provides user data for targeting
 */
@interface APDUserInfo : NSObject <APDUserInfo>
/**
 Set user ID
 */
@property (copy, nonatomic) NSString *userId;
/**
 Set user age
 */
@property (assign, nonatomic) NSUInteger age;
/**
 Set user gender
 */
@property (assign, nonatomic) APDUserGender gender;
=======
 *  Instance of class provides user data for targeting
 */
@interface APDUserInfo : NSObject <APDUserInfo>


/**
 *  Set user email
 */
@property (copy, nonatomic) NSString *email;

/**
 *  Set user ID
 */
@property (copy, nonatomic) NSString *userId;

/**
 *  Array of user interests in string
 * @code userInfo.interests = @[@"music", @"sport"];
 */
@property (copy, nonatomic) NSArray *interests;

/**
 *  Set user birthday
 */
@property (strong, nonatomic) NSDate *birthday;

/**
 *  Set user age
 */
@property (assign, nonatomic) NSUInteger age;

/**
 *  Set user gender
 */
@property (assign, nonatomic) APDUserGender gender;

/**
 *  Set user occupation
 */
@property (assign, nonatomic) APDUserOccupation occupation;

/**
 *  Set user relationship
 */
@property (assign, nonatomic) APDUserRelationship relationship;

/**
 *  Set user smoking attitude
 */
@property (assign, nonatomic) APDUserSmokingAttitude smokingAttitude;

/**
 *  Set user alcohol attitude
 */
@property (assign, nonatomic) APDUserAlcoholAttitude alcoholAttitude;

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
@end
