//
//  APDNativeAdView.h
//  Appodeal
//
<<<<<<< HEAD
//  Created by Stas Kochkin on 22/06/2019.
//  Copyright © 2019 Appodeal, Inc. All rights reserved.
=======
//  Created by Stas Kochkin on 22/06/2018.
//  Copyright © 2018 Appodeal, Inc. All rights reserved.
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
//

#import <UIKit/UIKit.h>


@protocol APDNativeAdView <NSObject>

- (nonnull UILabel *)titleLabel;
- (nonnull UILabel *)callToActionLabel;

@optional

- (nonnull UILabel *)descriptionLabel;
- (nonnull UIImageView *)iconView;
- (nonnull UIView *)mediaContainerView;
- (nonnull UILabel *)contentRatingLabel;
- (nonnull UIView *)adChoicesView;

- (void)setRating:(nonnull NSNumber *)rating;

+ (nonnull UINib *)nib;

@end
