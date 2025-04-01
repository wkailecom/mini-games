//
//  iOSBridage.h
//  TESTSDK
//
//  Created by wanghaiqi on 2021/12/9.
//

#import <Foundation/Foundation.h>
#import "AdManagerInitOption.h"

NS_ASSUME_NONNULL_BEGIN

@interface iOSBridge  : NSObject<AdManagerInitOption>

+ (iOSBridge *)share;

@end

NS_ASSUME_NONNULL_END
