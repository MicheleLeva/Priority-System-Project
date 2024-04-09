function X = main_hdrvdp_quality ()

root = "test";
dirs = [root+"/Test-4-SP", root+"/Test-4-AOI"];

% Display parameters
Y_peak = 200;     % Peak luminance in cd/m^2 (the same as nit)
contrast = 1000;  % Display contrast 1000:1
gamma = 2.2;      % Standard gamma-encoding
E_ambient = 100;  % Ambient light = 100 lux

full = double(imread('test\full.png')) / (2^8-1);
full = hdrvdp_gog_display_model(full, Y_peak, contrast, gamma, E_ambient );

pat = digitsPattern;

for i = 1:length(dirs)
    f = figure('visible','off');
    %X = zeros(46, 1);
    d1 = dirs(i);
    X = zeros(46, 2);
    mbps = ["/4", "/10", "/20", "/40"];
    %mbps = "/full";
    for k = 1:length(mbps)
        d = d1 + mbps(k) + "/SCREEN/";
        %mkdir(d + "/dE2000");
        imgs = dir(d + "/screen*.png");
        imgs = {imgs(:).name};
        [imgs, ~] = sort_nat(imgs);
        if exist(d1 + mbps(k) + "/vdp-hdr-quality", 'dir')
            rmdir(d1 + mbps(k) + "/vdp-hdr-quality", 's');
        end
        mkdir(d1 + mbps(k) + "/vdp-hdr-quality");
        for j = 1:length(imgs)
            seconds = join(extract(imgs(j), pat), '.');
            X(j, 1) = str2double(seconds);
            %img = double(imread(d + imgs(j)));
            img = double(imread(d + imgs(j))) / (2^8-1);
            img = hdrvdp_gog_display_model(img, Y_peak, contrast, gamma, E_ambient );
            diff = hdrvdp3('quality', img, full, 'rgb-native', 30, []);
            imagesc(diff.Q);
            %colormap gray;
            clim([0,1]);
            colorbar;
            saveas(f, d1 + mbps(k) + "/vdp-hdr-quality/" + imgs(j));
            %X(j) = mean(diff.P_map, "all");
            X(j, 2) = diff.Q;
            disp(dirs(i) + " - Directory " + mbps(k) + " ->" + j + ": " + imgs(j) + ": " + X(j));
        end
        plot(X)
        delete(d1 + mbps(k) + "/vdp-hdr-quality.png");
        delete(d1 + mbps(k) + "/vdp-hdr-quality.xlsx");
        saveas(f, d1 + mbps(k) + "/vdp-hdr-quality.png");
        writematrix(X, d1 + mbps(k) + "/vdp-hdr-quality.xlsx");
    end
end
