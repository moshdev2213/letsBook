package com.example.letsbook.ApiRoutes;

import com.example.letsbook.ModalDao.UpdatedUser;

import retrofit2.Call;
import retrofit2.http.Header;
import retrofit2.http.PATCH;
import retrofit2.http.Path;

public interface EditUserApi {
    @PATCH("/api/collections/users/records/{:id}")
    Call<UpdatedUser> updateUserDetails(
            @Header("Authorization") String authorization,
            @Path("id") String id
    );
}
