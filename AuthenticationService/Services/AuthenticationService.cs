using AuthenticationService.Interfaces;
using DB.DTO;
using DB.Entities;
using MessageQueue;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Security;
using static GrpcProvider.Protos.GrpcProvider;
using static Utility.Common;

namespace AuthenticationService.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly GrpcProviderClient _grpcClient;
        private readonly IRabbitMQPublisher<object> authenticationServicePublisher;
        private readonly IConfiguration _configuration;

        private readonly ServerInfoSetting _serverInfoSetting;

        public AuthenticationService(GrpcProviderClient grpcClient,IRabbitMQPublisher<object> authenticationServicePublisher, IOptions<ServerInfoSetting> serverInfoSetting)
        {
            this.authenticationServicePublisher = authenticationServicePublisher;
            this._serverInfoSetting = serverInfoSetting.Value;
            this._grpcClient = grpcClient;
        }

        public async Task<User> GetUser(UserLoginDTO userCreateModel) {

            //var result=await authenticationServicePublisher.PublishMessageAsyncWithQueue(userCreateModel, RabbitMQQueues.UserServiceQueue, "FindUserByUserCreateModel");



            userCreateModel.Password = Utility.Common.HashData(userCreateModel.Password);

            var result = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
            {
                ServiceName="UserService",
                FullClassName = "UserService.Services.UserService",
                Funct = "FindUserByUserCreateModel",
                Data = JsonConvert.SerializeObject(userCreateModel)
            });
            User user = JsonConvert.DeserializeObject<User>(result.Data);
            if(user!=null) authenticationServicePublisher.PublishMessageAsyncWithQueue(user.UserName + " Logged In at server " + _serverInfoSetting.ServerName, RabbitMQQueues.LogServiceQueue, "LogInfo");
            return user;
        }

        public async Task<UserRefreshTokenDTO> CreateRefreshToken(string userID)
        {
            UserRefreshTokenDTO userTokenModel = new UserRefreshTokenDTO { UserID = userID, RefreshToken = JwtAuthentication.GenerateRefreshToken() };
            _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
            {
                ServiceName = "UserService",
                FullClassName = "UserService.Services.UserService",
                Funct = "SaveRefreshToken",
                Data = JsonConvert.SerializeObject(userTokenModel)
            });
            return userTokenModel;
        }

        public async Task<UserToken> GetRefreshToken(string userID)
        {
            var result = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
            {
                ServiceName = "UserService",
                FullClassName = "UserService.Services.UserService",
                Funct = "GetRefreshToken",
                Data = JsonConvert.SerializeObject(userID)
            });
            UserToken existedUserToken = JsonConvert.DeserializeObject<UserToken>(result.Data);
            return existedUserToken;
        }
    }
}
