jQuery(document).ready(function ($) {
    $(document).ready(function () {
        (function () {
            const second = 1000,
                minute = second * 60,
                hour = minute * 60,
                day = hour * 24;

            //I'm adding this section so I don't have to keep updating this pen every year :-)
            //remove this if you don't need it
            let today = new Date();
            const dd = String(today.getDate()).padStart(2, "0");
            const mm = String(today.getMonth() + 1).padStart(2, "0");
            const yyyy = today.getFullYear();
            const dayMonth = "09/23/";
            const dayMonthb4 = "10/24/";
            const dayMonthb5 = "10/24/"
            const dayMonthv2 = "10/24/"
            const dayMonthv3 = "10/24/"
            const dayMonthv4 = "10/24/"
            const daygroupV2 = "10/24/"
            const dayV2B3 = "10/24/"
            const dayV3B2 = "10/24/"


            let runday = dayMonth + yyyy + "  11:00";
            let buoc2 = dayMonth + yyyy + " 11:00";
            let buoc4 = dayMonthb4 + yyyy + "  09:31";
            let buoc5 = dayMonthb5 + yyyy + "  09:32";
            let vong2 = dayMonthv3 + yyyy + "  09:33";
            let vong3 = dayMonthv2 + yyyy + "  09:36";
            let vong4 = dayMonthv4 + yyyy + "  11:00";
            //nhóm vòng 2 bước 2
            let GRvong2 = daygroupV2 + yyyy + "  09:34";
            //vong 2 buoc 3
            let v2b3 = dayV2B3 + yyyy + " 09:35";
            //vong 3 buoc 2
            let v3b2 = dayV3B2 + yyyy + " 09:37";




            today = mm + "/" + dd + "/" + yyyy + " " + today.getHours() + ":00";



            const countDown = new Date(runday).getTime();
            const countDownB2 = new Date(buoc2).getTime();
            const countDownB4 = new Date(buoc4).getTime();
            const countDownB5 = new Date(buoc5).getTime();
            const countDownV2 = new Date(vong2).getTime();
            const countDownV3 = new Date(vong3).getTime();
            const countDownV4 = new Date(vong4).getTime();
            const countDownGV4 = new Date(GRvong2).getTime();
            const countDownB3V2 = new Date(v2b3).getTime();
            const countDownB2V3 = new Date(v3b2).getTime();




            const registerButton = document.querySelector('.btn-dangky.regis-bottom a');
            const countdownElement = document.getElementById("countdown");
            const registerBtn = document.getElementById("registerBtn");
            const step1 = document.getElementById("buoc-1");
            const step1v2 = document.getElementById("buoc-1v2");
            const step1v3 = document.getElementById("buoc-1v3");
            const timestep1 = document.getElementById("time-buoc-1");
            const timestep1v2 = document.getElementById("time-buoc-1v2");
            const timestep1v3 = document.getElementById("time-buoc-1v3");
            const imgstep1 = document.getElementById("img-buoc-1");
            const imgstep1v2 = document.getElementById("img-buoc-1v2");
            const imgstep1v3 = document.getElementById("img-buoc-1v3");
            const step2 = document.getElementById("buoc-2");
            const step2v2 = document.getElementById("buoc-2v2");
            const step2v3 = document.getElementById("buoc-2v3");
            const timestep2 = document.getElementById("time-buoc-2");
            const timestep2v2 = document.getElementById("time-buoc-2v2");
            const timestep2v3 = document.getElementById("time-buoc-2v3");
            const imgstep2 = document.getElementById("img-buoc-2");
            const imgstep2v2 = document.getElementById("img-buoc-2v2");
            const imgstep2v3 = document.getElementById("img-buoc-2v3");
            const step3 = document.getElementById("buoc-3");
            const step3v2 = document.getElementById("buoc-3v2");
            const timestep3 = document.getElementById("time-buoc-3");
            const timestep3v2 = document.getElementById("time-buoc-3v2");
            const imgstep3 = document.getElementById("img-buoc-3");
            const imgstep3v2 = document.getElementById("img-buoc-3v2");
            const step4 = document.getElementById("buoc-4");
            const timestep4 = document.getElementById("time-buoc-4");
            const imgstep4 = document.getElementById("img-buoc-4");
            const step5 = document.getElementById("buoc-5");
            const timestep5 = document.getElementById("time-buoc-5");
            const imgstep5 = document.getElementById("img-buoc-5");
            const textdssv = document.getElementById("text-dssv");
            const textdssvb4 = document.getElementById("text-dssvb4");
            const textdssvb5 = document.getElementById("text-dssvb5");
            const textdsn = document.getElementById("text-dsn");
            const bxhcanhan = document.getElementById("bxh-canhan");
            const bxhtapthe = document.getElementById("bxh-tapthe");
            const dsb2 = document.getElementById("ds-buoc-2");
            const dsb4 = document.getElementById("ds-buoc-4");
            const dsb5 = document.getElementById("ds-buoc-5");
            const headernhom = document.getElementById("header-nhom");
            const dsnhom = document.getElementById("ds-nhom");
            const fulldsnhom = document.getElementById("fulldsnhom");
            const fullstep = document.getElementById("fullstep");
            const textvong1 = document.getElementById("vong1");
            const imgendv1 = document.getElementById("img-end-v1");
            const imgbophieu = document.getElementById("img-bophieu");
            const imgvong1 = document.getElementById("img-vong1");
            const textvong2 = document.getElementById("vong2");
            const imgvong2 = document.getElementById("img-vong2");
            const textvong3 = document.getElementById("vong3");
            const imgvong3 = document.getElementById("img-vong3");
            const textvong4 = document.getElementById("vong4");
            const imgvong4 = document.getElementById("img-vong4");


            if (step1v2) {
                step1v2.style.display = "none";
            }
            if (step1v3) {
                step1v3.style.display = "none";
            }
            if (step2v2) {
                step2v2.style.display = "none";
            }
            if (step2v3) {
                step2v3.style.display = "none";
            }
            if (step3v2) {
                step3v2.style.display = "none";
            }
            if (timestep1v2) {
                timestep1v2.style.display = "none";
            }
            if (timestep2v2) {
                timestep2v2.style.display = "none";
            }
            if (timestep3v2) {
                timestep3v2.style.display = "none";
            }
            if (timestep1v3) {
                timestep1v3.style.display = "none";
            }
            if (timestep2v3) {
                timestep2v3.style.display = "none";
            }
            if (imgstep1v2) {
                imgstep1v2.style.display = "none";
            }
            if (imgstep2v2) {
                imgstep2v2.style.display = "none";
            }
            if (imgstep3v2) {
                imgstep3v2.style.display = "none";
            }
            if (imgstep1v3) {
                imgstep1v3.style.display = "none";
            }
            if (imgstep2v3) {
                imgstep2v3.style.display = "none";
            }






            if (imgbophieu) {
                imgbophieu.style.display = "none";
            }

            if (imgendv1) {
                imgendv1.style.display = "none";
            }


            const x = setInterval(function () {
                const now = new Date().getTime();
                const distance = countDown - now;


                // Thực hiện các thao tác khi distance >= 0
                if (distance >= 0) {
                    const days = Math.floor(distance / (day));
                    const hours = Math.floor((distance % (day)) / (hour));
                    const minutes = Math.floor((distance % (hour)) / (minute));
                    const seconds = Math.floor((distance % (minute)) / second);
                    if(document.getElementById("days")) document.getElementById("days").innerText = days;
                    if(document.getElementById("hours")) document.getElementById("hours").innerText = hours;
                    if(document.getElementById("minutes")) document.getElementById("minutes").innerText = minutes;
                    if(document.getElementById("seconds")) document.getElementById("seconds").innerText = seconds;



                    if (textdssvb4) {
                        textdssvb4.style.display = "none";
                    }
                    if (textdssvb5) {
                        textdssvb5.style.display = "none";
                    }

                    if (registerButton) {
                        registerButton.removeAttribute('href');
                        registerButton.querySelector('p').innerText = 'Đăng ký tham gia';
                        registerButton.style.display = 'block';
                    }


                    if (countdownElement) {
                        countdownElement.style.display = 'flex';
                    }
                    //listsrcoll_right.style.display = "none";

                }

                // Thực hiện các thao tác khi distance < 0
                if (distance < 0) {
                    const distanceb4 = countDownB4 - now;
                    if (timestep2) {
                        timestep2.style.display = "none";
                    }
                    if (textdssv) {
                        textdssv.style.display = "none";
                    }

                    // document.getElementById("content").style.display = "block";

                    if (registerButton) {
                        registerButton.href = '/dang-ky-thong-tin';
                        registerButton.querySelector('p').innerText = 'Đăng ký tham gia';
                        registerButton.style.display = 'block';
                    }


                    if (registerBtn) {
                        registerBtn.style.display = "none";
                    }

                    //topSV.style.display = "block"
                    //sec11.style.display = "none";
                    //vong1.style.display = "none";
                    if (distanceb4 >= 0) {
                        const daysb4 = Math.floor(distanceb4 / (day));
                        const hoursb4 = Math.floor((distanceb4 % (day)) / (hour));
                        const minutesb4 = Math.floor((distanceb4 % (hour)) / (minute));
                        const secondsb4 = Math.floor((distanceb4 % (minute)) / second);
                        if(document.getElementById("days")) document.getElementById("days").innerText = daysb4;
                        if(document.getElementById("hours")) document.getElementById("hours").innerText = hoursb4;
                        if(document.getElementById("minutes")) document.getElementById("minutes").innerText = minutesb4;
                        if(document.getElementById("seconds")) document.getElementById("seconds").innerText = secondsb4;

                        if (textdssv) {
                            textdssv.style.display = "block"
                        }
                        if (textdssvb4) {
                            textdssvb4.style.display = "none"
                        }
                        if (textdssvb5) {
                            textdssvb5.style.display = "none"
                        }
                        if (imgstep3) {
                            imgstep3.style.display = "none";
                        }
                        /* an bxh */
                        if (bxhcanhan) {
                            bxhcanhan.style.display = "none";
                        }
                        if (bxhtapthe) {
                            bxhtapthe.style.display = "none";
                        }
                        /* an bxh */
                        /* an ds nhom */
                        if (headernhom) {
                            headernhom.style.display = "none";
                        }
                        if (dsnhom) {
                            dsnhom.style.display = "none";
                        }
                        if (textdsn) {
                            textdsn.style.display = "none"
                        }
                        /* an ds nhom */
                        /*an buoc 4*/
                        if (step4) {
                            step4.style.display = "none";
                        }
                        if (timestep4) {
                            timestep4.style.display = "none";
                        }
                        if (imgstep4) {
                            imgstep4.style.display = "none";
                        }
                        /*an buoc 4*/
                        /*an buoc 5*/
                        if (step5) {
                            step5.style.display = "none";
                        }
                        if (timestep5) {
                            timestep5.style.display = "none";
                        }
                        if (imgstep5) {
                            imgstep5.style.display = "none";
                        }
                        /*an buoc 5*/
                        if (dsb4) {
                            dsb4.style.display = "none";
                        }
                        if (dsb5) {
                            dsb5.style.display = "none";
                        }
                    }
                    if (distanceb4 < 0) {
                        const distanceb5 = countDownB5 - now;

                        if (timestep2) {
                            timestep2.style.display = "none";
                        }
                        if (timestep3) {
                            timestep3.style.display = "none";
                        }
                        if (textdssvb4) {
                            textdssvb4.style.display = "block"
                        }
                        if (textdssvb5) {
                            textdssvb5.style.display = "none"
                        }
                        /* an bxh */
                        if (bxhcanhan) {
                            bxhcanhan.style.display = "none";
                        }
                        if (bxhtapthe) {
                            bxhtapthe.style.display = "none";
                        }
                        /* an bxh */
                        if (headernhom) {
                            headernhom.style.display = "none";
                        }
                        if (dsnhom) {
                            dsnhom.style.display = "none";
                        }
                        if (textdsn) {
                            textdsn.style.display = "none"
                        }

                        if (imgstep3) {
                            imgstep3.style.display = "block";
                        }
                        if (dsb2) {
                            dsb2.style.display = "none";
                        }
                        if (dsb4) {
                            dsb4.style.display = "flex";
                        }
                        /*an buoc 4*/
                        if (step4) {
                            step4.style.display = "flex";
                        }
                        if (timestep4) {
                            timestep4.style.display = "block";
                        }
                        if (imgstep4) {
                            imgstep4.style.display = "none";
                        }
                        /*an buoc 4*/
                        /*an buoc 5*/
                        if (step5) {
                            step5.style.display = "none";
                        }
                        if (timestep5) {
                            timestep5.style.display = "none";
                        }
                        if (imgstep5) {
                            imgstep5.style.display = "none";
                        }
                        /*an buoc 5*/

                        if (distanceb5 >= 0) {
                            const daysb5 = Math.floor(distanceb5 / (day));
                            const hoursb5 = Math.floor((distanceb5 % (day)) / (hour));
                            const minutesb5 = Math.floor((distanceb5 % (hour)) / (minute));
                            const secondsb5 = Math.floor((distanceb5 % (minute)) / second);
                            document.getElementById("days").innerText = daysb5;
                            document.getElementById("hours").innerText = hoursb5;
                            document.getElementById("minutes").innerText = minutesb5;
                            document.getElementById("seconds").innerText = secondsb5;
                            /*an buoc 4*/
                            if (step4) {
                                step4.style.display = "flex";
                            }
                            if (timestep4) {
                                timestep4.style.display = "block";
                            }
                            if (imgstep4) {
                                imgstep4.style.display = "none";
                            }
                            /*an buoc 4*/
                            /*an buoc 5*/
                            if (step5) {
                                step5.style.display = "none";
                            }
                            if (timestep5) {
                                timestep5.style.display = "none";
                            }
                            if (imgstep5) {
                                imgstep5.style.display = "none";
                            }
                            /* an bxh */
                            if (bxhcanhan) {
                                bxhcanhan.style.display = "none";
                            }
                            if (bxhtapthe) {
                                bxhtapthe.style.display = "none";
                            }
                            /* an bxh */
                            if (headernhom) {
                                headernhom.style.display = "none";
                            }
                            if (dsnhom) {
                                dsnhom.style.display = "none";
                            }
                            if (textdsn) {
                                textdsn.style.display = "none"
                            }
                        }
                        if (distanceb5 < 0) {
                            const distancev2 = countDownV2 - now;

                            if (timestep4) {
                                timestep4.style.display = "none";
                            }
                            if (textdssvb4) {
                                textdssvb4.style.display = "none"
                            }
                            if (textdssvb5) {
                                textdssvb5.style.display = "block"
                            }

                            /* an bxh */
                            if (bxhcanhan) {
                                bxhcanhan.style.display = "none";
                            }
                            if (bxhtapthe) {
                                bxhtapthe.style.display = "none";
                            }
                            /* an bxh */

                            /*an buoc 4*/
                            if (step4) {
                                step4.style.display = "flex";
                            }
                            if (timestep4) {
                                timestep4.style.display = "block";
                            }
                            if (imgstep4) {
                                imgstep4.style.display = "block";
                            }
                            /*an buoc 4*/
                            /*an buoc 5*/
                            if (step5) {
                                step5.style.display = "flex";
                            }
                            if (timestep5) {
                                timestep5.style.display = "block";
                            }
                            if (imgstep5) {
                                imgstep5.style.display = "none";
                            }

                            if (dsb4) {
                                dsb4.style.display = "none";
                            }
                            if (dsb5) {
                                dsb5.style.display = "flex";
                            }
                            if (headernhom) {
                                headernhom.style.display = "none";
                            }
                            if (dsnhom) {
                                dsnhom.style.display = "none";
                            }
                            if (textdsn) {
                                textdsn.style.display = "none"
                            }
                            // bước 5
                            if (distancev2 >= 0) {
                                const daysv2 = Math.floor(distancev2 / (day));
                                const hoursv2 = Math.floor((distancev2 % (day)) / (hour));
                                const minutesv2 = Math.floor((distancev2 % (hour)) / (minute));
                                const secondsv2 = Math.floor((distancev2 % (minute)) / second);
                                document.getElementById("days").innerText = daysv2;
                                document.getElementById("hours").innerText = hoursv2;
                                document.getElementById("minutes").innerText = minutesv2;
                                document.getElementById("seconds").innerText = secondsv2;
                                /* an bxh */
                                if (timestep4) {
                                    timestep4.style.display = "none";
                                }

                                if (bxhcanhan) {
                                    bxhcanhan.style.display = "none";
                                }
                                if (bxhtapthe) {
                                    bxhtapthe.style.display = "none";
                                }
                                /* an bxh */
                                if (headernhom) {
                                    headernhom.style.display = "none";
                                }
                                if (dsnhom) {
                                    dsnhom.style.display = "none";
                                }
                                if (textdsn) {
                                    textdsn.style.display = "none";
                                }
                            }
                            //bat dau vong 2
                            if (distancev2 < 0) {
                                //vong 2 buoc 1
                                const distancev2b2 = countDownGV4 - now;
                                const daysv2b2 = Math.floor(distancev2b2 / (day));
                                const hoursv2b2 = Math.floor((distancev2b2 % (day)) / (hour));
                                const minutesv2b2 = Math.floor((distancev2b2 % (hour)) / (minute));
                                const secondsv2b2 = Math.floor((distancev2b2 % (minute)) / second);
                                document.getElementById("days").innerText = daysv2b2;
                                document.getElementById("hours").innerText = hoursv2b2;
                                document.getElementById("minutes").innerText = minutesv2b2;
                                document.getElementById("seconds").innerText = secondsv2b2;

                                if (step1v2) {
                                    step1v2.style.display = "flex";
                                }
                                if (timestep1v2) {
                                    timestep1v2.style.display = "block";
                                }
                                if (fulldsnhom) {
                                    fulldsnhom.style.display = "none";
                                }
                                if (textdssvb5) {
                                    textdssvb5.style.display = "none";
                                }
                                if (dsb5) {
                                    dsb5.style.display = "none";
                                }
                                if (imgvong1) {
                                    imgvong1.style.display = "block";
                                }
                                if (textvong2) {
                                    textvong2.style.display = "flex";
                                }
                                /*an buoc 4*/
                                if (step4) {
                                    step4.style.display = "none";
                                }
                                if (timestep4) {
                                    timestep4.style.display = "none";
                                }
                                if (imgstep4) {
                                    imgstep4.style.display = "none";
                                }
                                /*an buoc 4*/
                                /*an buoc 5*/
                                if (step5) {
                                    step5.style.display = "none";
                                }
                                if (timestep5) {
                                    timestep5.style.display = "none";
                                }
                                if (imgstep5) {
                                    imgstep5.style.display = "none";
                                }
                                if (step1) {
                                    step1.style.display = "none";
                                }
                                if (timestep1) {
                                    timestep1.style.display = "none";
                                }
                                if (imgstep1) {
                                    imgstep1.style.display = "none";
                                }

                                if (step2) {
                                    step2.style.display = "none";
                                }
                                if (timestep2) {
                                    timestep2.style.display = "none";
                                }
                                if (imgstep2) {
                                    imgstep2.style.display = "none";
                                }
                                if (step3) {
                                    step3.style.display = "none";
                                }
                                if (timestep3) {
                                    timestep3.style.display = "none";
                                }
                                if (imgstep3) {
                                    imgstep3.style.display = "none";
                                }
                                if (step4) {
                                    step1.style.display = "none";
                                }
                                if (timestep4) {
                                    timestep4.style.display = "none";
                                }
                                if (imgstep4) {
                                    imgstep4.style.display = "none";
                                }
                                if (step5) {
                                    step1.style.display = "none";
                                }
                                if (timestep5) {
                                    timestep1.style.display = "none";
                                }

                                if (imgendv1) {
                                    imgendv1.style.display = "block";
                                }

                                if (textdssvb4) {
                                    textdssvb4.style.display = "none";
                                }
                                if (textdssv) {
                                    textdssv.style.display = "none";
                                }
                                if (imgstep5) {
                                    imgstep5.style.display = "block";
                                }
                                /* an bxh */
                                if (bxhcanhan) {
                                    bxhcanhan.style.display = "none";
                                }
                                if (bxhtapthe) {
                                    bxhtapthe.style.display = "none";
                                }
                                if (dsb4) {
                                    dsb4.style.display = "none";
                                }
                                /* an bxh */
                                //vong 2 buoc 2
                                if (countDownGV4 < now) {
                                    const distancev2b3 = countDownB3V2 - now;
                                    const daysv2b3 = Math.floor(distancev2b3 / (day));
                                    const hoursv2b3 = Math.floor((distancev2b3 % (day)) / (hour));
                                    const minutesv2b3 = Math.floor((distancev2b3 % (hour)) / (minute));
                                    const secondsv2b3 = Math.floor((distancev2b3 % (minute)) / second);
                                    document.getElementById("days").innerText = daysv2b3;
                                    document.getElementById("hours").innerText = hoursv2b3;
                                    document.getElementById("minutes").innerText = minutesv2b3;
                                    document.getElementById("seconds").innerText = secondsv2b3;
                                    if (distancev2b3 < 0) {
                                        var distancev3 = countDownV3 - now;
                                    }
                                    if (fulldsnhom) {
                                        fulldsnhom.style.display = "block";
                                    }
                                    if (step1v2) {
                                        step1v2.style.display = "flex";
                                    }
                                    if (imgstep1v2) {
                                        imgstep1v2.style.display = "block";
                                    }
                                    if (timestep1v2) {
                                        timestep1v2.style.display = "none";
                                    }
                                    if (step2v2) {
                                        step2v2.style.display = "flex";
                                    }
                                    if (timestep2v2) {
                                        timestep2v2.style.display = "block";
                                    }
                                    if (textdsn) {
                                        textdsn.style.display = "block";
                                    }
                                    if (headernhom) {
                                        headernhom.style.display = "block";
                                    }

                                    if (textdssvb5) {
                                        textdssvb5.style.display = "none";
                                    }
                                    if (dsnhom) {
                                        dsnhom.style.display = "flex";
                                    }

                                    if (dsb5) {
                                        dsb5.style.display = "none";
                                    }
                                    //vong 2 buoc 3
                                    if (countDownB3V2 < now) {
                                        const daysv3 = Math.floor(distancev3 / (day));
                                        const hoursv3 = Math.floor((distancev3 % (day)) / (hour));
                                        const minutesv3 = Math.floor((distancev3 % (hour)) / (minute));
                                        const secondsv3 = Math.floor((distancev3 % (minute)) / second);
                                        document.getElementById("days").innerText = daysv3;
                                        document.getElementById("hours").innerText = hoursv3;
                                        document.getElementById("minutes").innerText = minutesv3;
                                        document.getElementById("seconds").innerText = secondsv3;
                                        if (fulldsnhom) {
                                            fulldsnhom.style.display = "none";
                                        }
                                        if (textdsn) {
                                            textdsn.style.display = "none";
                                        }
                                        if (headernhom) {
                                            headernhom.style.display = "none";
                                        }
                                        if (dsnhom) {
                                            dsnhom.style.display = "none";
                                        }
                                        if (timestep2v2) {
                                            timestep2v2.style.display = "none";
                                        }
                                        if (imgstep2v2) {
                                            imgstep2v2.style.display = "block"
                                        }
                                        if (step3v2) {
                                            step3v2.style.display = "flex";
                                        }
                                        if (timestep3v2) {
                                            timestep3v2.style.display = "block";
                                        }
                                    }

                                }

                                //bat dau vong 3
                                if (distancev3 < 0) {
                                    //vong 3 buoc 1
                                    const distancev3b2 = countDownB2V3 - now;
                                    const daysv3b2 = Math.floor(distancev3b2 / (day));
                                    const hoursv3b2 = Math.floor((distancev3b2 % (day)) / (hour));
                                    const minutesv3b2 = Math.floor((distancev3b2 % (hour)) / (minute));
                                    const secondsv3b2 = Math.floor((distancev3b2 % (minute)) / second);
                                    document.getElementById("days").innerText = daysv3b2;
                                    document.getElementById("hours").innerText = hoursv3b2;
                                    document.getElementById("minutes").innerText = minutesv3b2;
                                    document.getElementById("seconds").innerText = secondsv3b2;


                                    if (step1v2) {
                                        step1v2.style.display = "none";
                                    }
                                    if (imgstep1v2) {
                                        imgstep1v2.style.display = "none";
                                    }
                                    if (step2v2) {
                                        step2v2.style.display = "none";
                                    }
                                    if (imgstep2v2) {
                                        imgstep2v2.style.display = "none";
                                    }
                                    if (step3v2) {
                                        step3v2.style.display = "none";
                                    }
                                    if (timestep3v2) {
                                        timestep3v2.style.display = "none";
                                    }
                                    if (imgstep3v2) {
                                        imgstep3v2.style.display = "none";
                                    }

                                    if (step1v3) {
                                        step1v3.style.display = "flex";
                                    }
                                    if (timestep1v3) {
                                        timestep1v3.style.display = "block";
                                    }

                                    if (imgvong2) {
                                        imgvong2.style.display = "block";
                                    }
                                    if (textvong3) {
                                        textvong3.style.display = "flex";
                                    }

                                    //vong 3 buoc 2
                                    if (countDownB2V3 < now) {
                                        var distancev4 = countDownV4 - now;
                                        if (step2v3) {
                                            step2v3.style.display = "flex";
                                        }
                                        if (timestep2v3) {
                                            timestep2v3.style.display = "block";
                                        }
                                        if (timestep1v3) {
                                            timestep1v3.style.display = "none";
                                        }
                                        if (imgstep1v3) {
                                            imgstep1v3.style.display = "block";
                                        }
                                        if (distancev4 >= 0) {
                                            const daysv4 = Math.floor(distancev4 / (day));
                                            const hoursv4 = Math.floor((distancev4 % (day)) / (hour));
                                            const minutesv4 = Math.floor((distancev4 % (hour)) / (minute));
                                            const secondsv4 = Math.floor((distancev4 % (minute)) / second);
                                            document.getElementById("days").innerText = daysv4;
                                            document.getElementById("hours").innerText = hoursv4;
                                            document.getElementById("minutes").innerText = minutesv4;
                                            document.getElementById("seconds").innerText = secondsv4;
                                            if (textvong4) {
                                                textvong4.style.display = "block";
                                            }
                                        }
                                        if (distancev4 < 0) {

                                            if (countdownElement) {
                                                countdownElement.style.display = "none";
                                            }
                                            if (step1v2) {
                                                step1v2.style.display = "none";
                                            }
                                            if (step1v3) {
                                                step1v3.style.display = "none";
                                            }
                                            if (step2v2) {
                                                step2v2.style.display = "none";
                                            }
                                            if (step2v3) {
                                                step2v3.style.display = "none";
                                            }
                                            if (step3v2) {
                                                step3v2.style.display = "none";
                                            }
                                            if (imgstep1v2) {
                                                imgstep1v2.style.display = "none";
                                            }
                                            if (imgstep1v3) {
                                                imgstep1v3.style.display = "none";
                                            }
                                            if (imgstep2v2) {
                                                imgstep2v2.style.display = "none";
                                            }
                                            if (imgstep2v3) {
                                                imgstep2v3.style.display = "none";
                                            }
                                            if (imgstep3v2) {
                                                imgstep3v2.style.display = "none";
                                            }
                                            if (textvong1) {
                                                textvong1.style.display = "none";
                                            }
                                            if (textvong2) {
                                                textvong2.style.display = "none";
                                            }
                                            if (textvong3) {
                                                textvong3.style.display = "none";
                                            }
                                            if (textvong4) {
                                                textvong4.style.display = "none";
                                            }
                                            if (imgvong1) {
                                                imgvong1.style.display = "none";
                                            }
                                            if (imgvong2) {
                                                imgvong2.style.display = "none";
                                            }
                                            if (imgvong3) {
                                                imgvong3.style.display = "none";
                                            }
                                            if (fulldsnhom) {
                                                fulldsnhom.style.display = "none";
                                            }
                                            if (textdsn) {
                                                textdsn.style.display = "none";
                                            }
                                            if (fullstep) {
                                                fullstep.style.display = "none";
                                            }
                                            if (bxhcanhan) {
                                                bxhcanhan.style.display = "block";
                                            }
                                            if (bxhtapthe) {
                                                bxhtapthe.style.display = "block";
                                            }
                                        }
                                    }


                                }
                            }
                        }
                    }
                }



            }, 0);
        }());
        $('.step-other').click(function () {
            $(this).next('.block-show').slideToggle();
            $(this).parent('.col-inner').toggleClass('active');
            $(this).parent('.for-xemthem').toggleClass('for-q');
        })
        $('.home .section-1').css('visibility', 'visible');
        //Setting sticky menu
        $(window).scroll(function () {
            var sticky = $('#wide-nav');
            var scroll = $(window).scrollTop();
            var topBar = $('#top-bar').innerHeight();
            var mainMenu = $('#masthead').innerHeight();
            var wideNav = $('#wide-nav').innerHeight();
            var isScroll = topBar + mainMenu + wideNav;
            if (scroll >= isScroll) {
                sticky.addClass('sticky');
            } else {
                sticky.removeClass('sticky');
            }
        });

        $('.info-client').click(function () {
            $('.dropdown-action').slideToggle();
        });

        // Xử lý menu mobile
        if ($(window).width() < 550) {
            $('.navbar-toggler').click(function () {
                $(this).parent('.navbar-toggler').hide();
                $('.overlay_menu').addClass('active');
                $('.mobile_menu .navbar-collapse').addClass('show');
            })
            $('#navbarCollapse > .navbar-toggler').click(function () {
                $('.header-inner > .navbar-toggler').show();
                $('.mobile_menu .navbar-collapse').removeClass('show');
                $('.overlay_menu').removeClass('active');
            });
            $('.mobile_menu .has-child .fa-angle-down').click(function () {
                $(this).next('.nav-mobile-sub').slideToggle();
                $(this).toggleClass('rotate_180');
            });
            $(document).on('click', '.overlay_menu.active', function (event) {
                $('.header-inner > .navbar-toggler').show();
                $('.mobile_menu .navbar-collapse').removeClass('show');
                $(this).removeClass('active');
            });
            $('.mobile_menu .has-child .angle-down').click(function () {
                $(this).next('.nav-mobile-sub').slideToggle();
                $(this).toggleClass('rotate_180');
            });
        }

        // Change type input
        $('#basic-addon2').click(function () {
            if ($(this).prev('input').attr('type') == 'text') {
                $(this).prev('input').prop("type", "password");
                $(this).html('<i class="fa-sharp fa-regular fa-eye-slash"></i>');
            } else {
                $(this).prev('input').prop("type", "text");
                $(this).html('<i class="fa-sharp fa-regular fa-eye"></i>');
            }

        });
        $('#basic-addon1').click(function () {
            if ($(this).prev('input').attr('type') == 'text') {
                $(this).prev('input').prop("type", "password");
                $(this).html('<i class="fa-sharp fa-regular fa-eye-slash"></i>');
            } else {
                $(this).prev('input').prop("type", "text");
                $(this).html('<i class="fa-sharp fa-regular fa-eye"></i>');
            }

        });

        // Nav sidebar info user
        $('.nav-tab-vertical > .item > a.has-child').click(function () {
            $(this).next('.list-child-tab').slideToggle();
            $(this).toggleClass('rotate');
        });
        // Editor button
        $('.main-field .input-group input').attr('disabled', 'disabled');
        $('.main-field .input-group select').attr('disabled', 'disabled');
        $('.main-field .input-group textarea').attr('disabled', 'disabled');
        //$('.btn-editor > a.editor-click').click(function(){
        //    $('.btn-editor').hide();
        //    $('.btn-action-editor').show();
        //    $('.main-field .input-group input').removeAttr("disabled");
        //    $('.main-field .input-group select').removeAttr("disabled");
        //    $('.main-field .input-group textarea').removeAttr("disabled");
        //})
        //$('.editor-close').click(function(){
        //    $('.btn-action-editor').hide();
        //    $('.btn-editor').show();
        //    $('.main-field .input-group input').attr('disabled', 'disabled');
        //    $('.main-field .input-group select').attr('disabled', 'disabled');
        //    $('.main-field .input-group textarea').attr('disabled', 'disabled');
        //})
        $('.head-faq').click(function () {
            $(this).next('.main-faq').slideToggle();
        })
        //Setting select 2
        jQuery('#select').on('select2:open', function (e) {
            jQuery('.select2-dropdown').hide();
            setTimeout(function () { jQuery('.select2-dropdown').slideDown("slow", "easeInOutQuint"); }, 200);
        });
        $("#chonse-language").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#sex").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#country").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#typevia").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#chuc-danh").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#hoc-ham").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#hoc-vi").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#linh-vuc-nghien-cuu").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#co-quan").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#city").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#quan-huyen").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#phuong-xa").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#co-quan-chu-quan").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#typeS").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#orderAr").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#type-data-1").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#type-data-2").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#type-data-3").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#type-data-4").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#pvud").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#lvnc").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#co-quan-to-chuc").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#nhom-quyen").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        $("#UnitDk").select2(
            {
                dropdownPosition: 'below',
                dropdownCssClass: 'no-search',
                minimumResultsForSearch: -1
            }
        );
        // Pick date
        if ($('#birthday')) {
            $('#birthday').dateTimePicker({
                mode: 'date',
                format: 'dd/MM/yyyy',
                yearName: 'Năm',
                monthName: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            });
        }
        if ($('#dateactive')) {
            $('#dateactive').dateTimePicker({
                mode: 'date',
                format: 'dd/MM/yyyy',
                yearName: 'Năm',
                monthName: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            });
        }
        if ($('#tu_ngay')) {
            $('#tu_ngay').dateTimePicker({
                mode: 'date',
                format: 'dd/MM/yyyy',
                yearName: 'Năm',
                monthName: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            });
        }
        if ($('#den_ngay')) {
            $('#den_ngay').dateTimePicker({
                mode: 'date',
                format: 'dd/MM/yyyy',
                yearName: 'Năm',
                monthName: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            });
        }
        if ($('#created-date')) {
            $('#created-date').dateTimePicker({
                mode: 'date',
                format: 'dd/MM/yyyy',
                yearName: 'Năm',
                monthName: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            });
        }
        //if ($("#ListFaqs")) {
        //    $("#ListFaqs").load("/Home/ListFaqs");
        //}
        //if ($("#ListDataTq")) {
        //    $("#ListDataTq").load("/Home/ListDataTq");
        //}
        //if ($("#ListDataAll")) {
        //    $("#ListDataAll").load("/Home/ListDataAll");
        //}

        //$(document).on('click', '.tree label', function (e) {
        //    $(this).next('ul').fadeToggle();
        //    e.stopPropagation();
        //});

        $(document).on('change', '.tree input[type=checkbox]', function (e) {
            $(this).siblings('ul').find("input[type='checkbox']").prop('checked', this.checked);
            $(this).parentsUntil('.tree').children("input[type='checkbox']").prop('checked', this.checked);
            e.stopPropagation();
        });

        $(document).on('click', 'button', function (e) {
            switch ($(this).text()) {
                case 'Collepsed':
                    $('.tree ul').fadeOut();
                    break;
                case 'Expanded':
                    $('.tree ul').fadeIn();
                    break;
                case 'Checked All':
                    $(".tree input[type='checkbox']").prop('checked', true);
                    break;
                case 'Unchek All':
                    $(".tree input[type='checkbox']").prop('checked', false);
                    break;
                default:
            }
        });
    });
});