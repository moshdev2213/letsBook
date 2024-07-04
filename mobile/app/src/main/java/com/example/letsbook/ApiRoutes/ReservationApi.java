package com.example.letsbook.ApiRoutes;

import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.ReservationRes;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.Path;

public interface ReservationApi {
    @POST("/api/collections/reservation/records")
    Call<ReservationReq> createReservation(
            @Header("Authorization") String authorization,
            @Body ReservationReq reservationReq
    );
    @GET("/api/collections/reservation/records")
    Call<ReservationRes> getAllReservations(
            @Header("Authorization") String authorization
    );
    @PATCH("/api/collections/reservation/records/{id}")
    Call<ReservationItem> cancelReservation(
            @Header("Authorization") String authorization,
            @Path("id") String id,
            @Body ReservationReq reservationReq
    );
    @PATCH("/api/collections/reservation/records/{id}")
    Call<ReservationItem> editReservation(
            @Header("Authorization") String authorization,
            @Path("id") String id,
            @Body ReservationReq reservationReq
    );
}
