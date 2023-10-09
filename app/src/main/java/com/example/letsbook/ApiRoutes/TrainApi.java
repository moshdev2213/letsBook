package com.example.letsbook.ApiRoutes;

import com.example.letsbook.Modal.User;
import com.example.letsbook.ModalDao.TrainRes;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.Query;

public interface TrainApi {
    @GET("/api/collections/train/records")
    Call<TrainRes> getAllTrain(
            @Header("Authorization") String authorization
    );
}
