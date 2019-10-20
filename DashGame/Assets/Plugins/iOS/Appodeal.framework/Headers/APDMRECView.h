//
//  APDMRECView.h
//  Appodeal
//
<<<<<<< HEAD
//  AppodealSDK 2.5.11
//
//  Copyright © 2019 Appodeal, Inc. All rights reserved.
=======
//  AppodealSDK 2.4.8.1-Beta
//
//  Copyright © 2018 Appodeal, Inc. All rights reserved.
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
//

#import <Appodeal/APDBannerView.h>

<<<<<<< HEAD
__attribute__((deprecated("This class is deprecated and will be removed in next release")))
/**
 Instance of this class returns rectangular banner of size 300x250
 All methods/properties besides initializer similar to APDBannerView
 */
@interface APDMRECView : APDBannerView
/**
 Use -init method to create instance APDMRECView
 @return Instance of APDMRECView
=======
/*!
 * Instance of this class returns rectangular banner of size 300x250
 * All methods/properties besides initializer similar to APDBannerView
 */

__attribute__((deprecated("This class is deprecated and will be removed in next release")))
@interface APDMRECView : APDBannerView

/*!
 *  Use -init method to create instance APDMRECView
 *
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
 */
- (instancetype)init;

@end
<<<<<<< HEAD
/**
 Compatibility alias APDMRECView
 */
=======

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
@compatibility_alias AppodealMRECView APDMRECView;
