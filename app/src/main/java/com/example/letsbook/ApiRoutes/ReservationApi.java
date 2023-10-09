package com.example.letsbook.ApiRoutes;

import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.ReservationRes;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Header;
import retrofit2.http.POST;

public interface ReservationApi {
    @POST("/api/collections/reservation/records")
    Call<ReservationReq> createReservation(
            @Header("Authorization") String authorization,
            @Body ReservationReq reservationReq
    );
}
