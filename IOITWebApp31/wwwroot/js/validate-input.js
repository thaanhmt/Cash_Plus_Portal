jQuery(document).ready(function ($) {
    $(document).ready(function () {
        // Validate Email field
        function validateEmail(element){
            $(element).each(function(){
                $('<span class="validate-true"><i class="fa-duotone fa-circle-check"></i></span><span class="validate-false"><i class="fa-duotone fa-circle-exclamation"></i></span>').insertAfter($(this));
                var placeholderCurent = $(this).attr('placeholder');
                const regex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                $(this).blur(function(){
                    if (regex.test($(this).val())) {
                        $(this).css('border','solid 1px #00497c');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').show();
                        $(this).next('.validate-true').next('.validate-false').hide();
                        $(this).addClass('is-check');
                        $(this).css('color','#00497c');
                    } else {
                        if($(this).val() == ""){
                            $(this).attr('placeholder','Trường này là bắt buộc');
                        }else{
                            $(this).attr('placeholder','Cần nhập đúng email');
                        }
                        $(this).css('border','solid 1px rgb(139 18 18)');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').hide();
                        $(this).next('.validate-true').next('.validate-false').show();
                        $(this).addClass('check-false');
                        $(this).val('');
                    }
                });
                $(this).focus(function() { 
                    $(this).css('border','solid 1px #00497c');
                    $(this).attr('placeholder',placeholderCurent);
                    $(this).css('color','#00497c');
                    $(this).removeClass('check-false');
                    $(this).next('.validate-true').css('display','none');
                    $(this).next('.validate-true').next('.validate-false').css('display','none');
                });
            });
        }
        // Validate NumberPhone field
        function validatePhone(element){
            $(element).each(function(){
                $('<span class="validate-true"><i class="fa-duotone fa-circle-check"></i></span><span class="validate-false"><i class="fa-duotone fa-circle-exclamation"></i></span>').insertAfter($(this));
                var placeholderCurent = $(this).attr('placeholder');
                const regex = /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/;
                $(this).blur(function(){
                    if (regex.test($(this).val())) {
                        $(this).css('border','solid 1px rgb(17 81 155)');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').show();
                        $(this).next('.validate-true').next('.validate-false').hide();
                        $(this).addClass('is-check');
                        $(this).css('color','#00497c');
                    } else {
                        if($(this).val() == ""){
                            $(this).attr('placeholder','Trường này là bắt buốc');
                        }else{
                            $(this).attr('placeholder','Cần nhập đúng số điện thoại');
                        }
                        $(this).css('border','solid 1px #EF0000');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').hide();
                        $(this).next('.validate-true').next('.validate-false').show();
                        $(this).addClass('check-false');
                        $(this).val('');
                    }
                });
                $(this).focus(function() { 
                    $(this).css('border','solid 1px #00497c');
                    $(this).attr('placeholder',placeholderCurent);
                    $(this).css('color','#00497c');
                    $(this).removeClass('check-false');
                    $(this).next('.validate-true').css('display','none');
                    $(this).next('.validate-true').next('.validate-false').css('display','none');
                });
            });
        }
        //Validate Required
        function validateRequired(element){
            $(element).each(function(){
                $('<span class="validate-true"><i class="fa-duotone fa-circle-check"></i></span><span class="validate-false"><i class="fa-duotone fa-circle-exclamation"></i></span>').insertAfter($(this));
                var placeholderCurent = $(this).attr('placeholder');
                $(this).blur(function(){
                    if($(this).val() == ""){
                        $(this).css('border','solid 1px rgb(139 95 18)');
                        $(this).css('color', '#00497c');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').hide();
                        $(this).next('.validate-true').next('.validate-false').show();
                        $(this).addClass('check-false');
                        if($(this).val() == ""){
                            $(this).attr('placeholder','Trường này là bắt buộc');
                        }
                    }else{
                        $(this).css('border','solid 1px #00497c');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').show();
                        $(this).next('.validate-true').next('.validate-false').hide();
                        $(this).addClass('is-check');
                        $(this).css('color','#00497c');
                    }
                });
                $(this).focus(function() { 
                    $(this).css('border','solid 1px #00497c');
                    $(this).attr('placeholder',placeholderCurent);
                    $(this).removeClass('check-false');
                    $(this).css('color','#00497c');
                    $(this).next('.validate-true').css('display','none');
                    $(this).next('.validate-true').next('.validate-false').css('display','none');
                });
            });
        }
        //Vailidate date field
        function validateDate(element){
            $(element).each(function(){
                $('<span class="validate-true"><i class="fa-duotone fa-circle-check"></i></span><span class="validate-false"><i class="fa-duotone fa-circle-exclamation"></i></span>').insertAfter($(this));
                var placeholderCurent = $(this).attr('placeholder');
                const regex = /^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$/;
                $(this).blur(function(){
                    if (regex.test($(this).val())) {
                        $(this).css('border','solid 1px #fff0');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').show();
                        $(this).next('.validate-true').next('.validate-false').hide();
                        $(this).addClass('is-check');
                        $(this).css('color','#00497c');
                    }else if($(this).val() == ""){
                        $(this).attr('placeholder',placeholderCurent);
                        $(this).css('border','solid 1px #fff0');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').hide();
                        $(this).next('.validate-true').next('.validate-false').hide();
                        $(this).addClass('is-check');
                        $(this).css('color','#00497c');
                    } else {
                        $(this).attr('placeholder','Nhập đúng ngày');
                        $(this).css('border','solid 1px #EF0000');
                        $(this).css('border-radius','5px');
                        $(this).next('.validate-true').hide();
                        $(this).next('.validate-true').next('.validate-false').show();
                        $(this).addClass('check-false');
                        $(this).val('');
                    }
                });
                $(this).focus(function() { 
                    $(this).css('border','solid 1px #00497c');
                    $(this).attr('placeholder',placeholderCurent);
                    $(this).css('color','#00497c');
                    $(this).removeClass('check-false');
                    $(this).next('.validate-true').css('display','none');
                    $(this).next('.validate-true').next('.validate-false').css('display','none');
                });
            });
        }
        validatePhone('.form-contact #numberphone');
        validateRequired('.form-contact #fullname');
        validateRequired('.form-contact #title');
        validateRequired('.form-contact #note');
        validateEmail('.form-contact #email');
    });
});