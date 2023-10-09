package com.example.letsbook.ApiRoutes;

import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.AuthPassEmail;
import com.example.letsbook.ModalDao.AuthSignUp;
import com.example.letsbook.ModalDao.AuthSignUpRes;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Header;
import retrofit2.http.POST;

public interface AuthApi {
    @POST("/api/collections/users/auth-with-password")
    Call<UserRecord> getUserAuth(@Body AuthPassEmail authPassEmail);

    @POST("/api/collections/users/records")
    Call<AuthSignUpRes> createUserAuth(@Body AuthSignUp authSignUp);

}
