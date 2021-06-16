//
//  AR_core_interface.h
//  FPointExtraction
//
//  Created by 4DAGE_iMacMini on 2020/12/15.
//  Copyright © 2020 Lei.FY. All rights reserved.
//

#import <Foundation/Foundation.h>
#include <vector>
#include <string>
#import <mach/mach.h>

using namespace std;

NS_ASSUME_NONNULL_BEGIN


@interface OrderData : NSObject

@property (nonatomic,assign)  int       index;

@property (nonatomic, strong) NSData    *data;

@end


@interface AR_core_interface : NSObject

+(instancetype)shareSingleton;

/// @brief init fuction，creat operation path,And  copy  xxx.bin xxx.feat  xxx.desc to givened path
/// @param operationPath  operation path ,nomal is iOS docment path
/// @param sfmARBinFilePath   the path you xxx.bin file
/// @param sfmARDescFilePath the path you xxx.feat file
/// @param sfmARfeatFilePath  the path you xxx.desc file

-(BOOL)creatOperationPath:(NSString *)operationPath
         sfmARBinFilePath:(NSString *)sfmARBinFilePath
        sfmARDescFilePath:(NSString *)sfmARDescFilePath
        sfmARfeatFilePath:(NSString *)sfmARfeatFilePath;



/// @brief set parm ,if all parm is checked right ,then prcess sfm image
/// @param position positon of  camera
/// @param rotation Four elements form arkti
/// @param focalLength focalLength
/// @param principalPoint principalPoint
/// @param resolution resolution of image
/// @param imageData   byte pointer of image data
//-(BOOL)imageLocalProcessWithPosition:(vector<float>)position
//                            rotation:(vector<float>)rotation
//                         focalLength:(vector<float>)focalLength
//                      principalPoint:(vector<float>)principalPoint
//                          resolution:(vector<int>)resolution
//                             arPoint:(vector<float>)arPoint
//                           imageData:(Byte *)imageData
//                          byteLength:(long)byteLength;


/// @brief  if sucess get the json result
-(NSString *)getARSFMResult;


/// @brief destroy ar_handler
-(void)close;

@end

///< unity调用接口
extern "C" {

///< ARSFM 初屎化

//参数结构

typedef struct
{
    float        x;
    float        y;
    float        z;
}AR_SFM_Point;

typedef struct
{
    float        x;
    float        y;
    float        z;
    float        w;
}AR_SFM_rotation;

typedef struct
{
    float        x;
    float        y;
}AR_SFM_focalLength;

typedef struct
{
    float        x;
    float        y;
    
}AR_SFM_principalPoint;

typedef struct
{
    int        x;
    int        y;
}AR_SFM_resolution;


BOOL  _iOS_init_FKARSFM(char *operationPath,char *sfmARBinFilePath,char *sfmARDescFilePath,char *sfmARfeatFilePath);

BOOL _iOS_process_FKARSFM(AR_SFM_Point          position,
                          AR_SFM_rotation       rotation,
                          AR_SFM_focalLength    focalLength,
                          AR_SFM_principalPoint principalPoint,
                          AR_SFM_resolution     resolution,
                          int                   method,
                          Byte*                 imageData,
                          long                  byteLength,
                          char*                 csys_x,
                          char*                 csys_y,
                          char*                 csys_z,
                          int                   platform);

BOOL _iOS_process_FKARSFM_RGB24(AR_SFM_Point               position,
                                AR_SFM_rotation            rotation,
                                AR_SFM_focalLength         focalLength,
                                AR_SFM_principalPoint      principalPoint,
                                AR_SFM_resolution          resolution,
                                int                        method,
                                Byte*                      imageData,
                                long                       byteLength,
                                char*                      csys_x,
                                char*                      csys_y,
                                char*                      csys_z,
                                int                        platform);


char* _iOS_get_result_FKARSFM();


void _iOS_close_FKARSFM();


}


NS_ASSUME_NONNULL_END
