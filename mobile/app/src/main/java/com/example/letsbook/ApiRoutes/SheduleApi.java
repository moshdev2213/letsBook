package com.example.letsbook.ApiRoutes;

import com.example.letsbook.ModalDao.SheduleRes;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.Query;

public interface SheduleApi {
    @GET("/api/collections/shedule/records")
    Call<SheduleRes> getAllShedules(
            @Query("filter") String shedule,
            @Query("fields") String id,
            @Header("Authorization") String authorization
    );
}
