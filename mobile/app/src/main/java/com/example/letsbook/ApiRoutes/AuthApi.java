package com.example.letsbook.ApiRoutes;

import com.example.letsbook.Modal.User;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.AuthPassEmail;
import com.example.letsbook.ModalDao.AuthSignUp;
import com.example.letsbook.ModalDao.AuthSignUpRes;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface AuthApi {
    @POST("/api/collections/users/auth-with-password")
    Call<UserRecord> getUserAuth(@Body AuthPassEmail authPassEmail);

    @POST("/api/collections/users/records")
    Call<AuthSignUpRes> createUserAuth(@Body AuthSignUp authSignUp);

    @GET("/api/collections/users/records")
    Call<User> getUserDetail(
            @Query("filter") String email,
            @Header("Authorization") String authorization
            );

}
